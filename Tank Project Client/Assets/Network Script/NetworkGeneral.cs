using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralSystem;

public class NetworkGeneral : MonoBehaviour
{
    public ClientManager clientManager;
    public List<(Vector3 position, Quaternion rotation)>[] revcMoveData;
    public List<Quaternion>[] revcRotData;
    public bool[] revcFireData;
    public bool[] revcChangeFireData;

    [SerializeField]
    private NetworkRecvInteract[] recvInteract;
    public void AddRecvInteract(NetworkRecvInteract interact,int ID)
    {
        if (clientManager == null) clientManager = FindAnyObjectByType<ClientManager>();
        if (recvInteract == null || recvInteract.Length != clientManager.clientQuanty+1)
        {
            recvInteract = new NetworkRecvInteract[clientManager.clientQuanty+1];
        }

        recvInteract[ID] = interact;
    }
    public void Init()
    {
        FindAnyObjectByType<UDPSender>().SetNetWorkGeneral(this);
        ///Khởi tạo revcStr
        clientManager = FindAnyObjectByType<ClientManager>();
        revcMoveData = new List<(Vector3 position, Quaternion rotation)>[clientManager.clientQuanty + 1];
        revcRotData = new List<Quaternion>[clientManager.clientQuanty + 1];
        revcFireData = new bool[clientManager.clientQuanty + 1];
        revcChangeFireData = new bool[clientManager.clientQuanty + 1];
        for (int i = 1; i < revcMoveData.Length; i++)
        {
            revcMoveData[i] = new();
            revcRotData[i] = new();
        }

        ///Khởi tạo
    }
    public void Awake()
    {
        Init();
    }
    //public void SetRevc(byte[] receivedData)
    //{
    //    int offset = 0;
    //    SetMoveRevc(DecodeTransformData(receivedData, ref offset), 1);
    //    Debug.Log("Dữ liệu off là: " + offset);
    //    SetMoveRevc(DecodeTransformData(receivedData, ref offset), 2);
    //    Debug.Log("Dữ liệu off 2 là: " + offset);
    //}

    public void RecvData(byte[] encodedData)
    {
        foreach (var (command, id, dataLength, data) in DecodeWithoutCheckByte(encodedData))
        {
            var offsetIn = 0;
            Debug.Log("Command:" + command + " id: " + id);
            switch ((Command)command)
            {
                case Command.Move:
                    SetMoveRevc(DecodeMoveData(data, ref offsetIn), id);
                    SetRotRevc(DecodeRotateData(data, ref offsetIn), id);
                    break;
                case Command.Fire:
                    revcFireData[id] = true;
                    //Debug.Log("Đã nhận được phản hồi: fire");
                    break;
                case Command.ChangeFire:
                    revcChangeFireData[id] = true;
                    //Debug.Log("Đã nhận được phản hồi: change fire");
                    break;
                case Command.Damage:
                    recvInteract[id].NetworkCallDamage(DecodeDamageData(data));
                    //Debug.Log("Đã nhận đc phản hồi damage của " + id);
                    break;
                default:
                    break;
            }
        }
    }
    public void SetMoveRevc(List<(Vector3 position, Quaternion rotation)> revcData, int id)
    {
        revcMoveData[id] = revcData;
    }
    public void SetRotRevc(List<Quaternion> revcData, int id)
    {
        revcRotData[id] = revcData;
    }
    public List<(Vector3 position, Quaternion rotation)> DecodeMoveData(byte[] receivedData, ref int offset)
    {
        List<(Vector3 position, Quaternion rotation)> result = new();

        // Kiểm tra dữ liệu nhận được có hợp lệ không
        if (receivedData == null || receivedData.Length < 28)
        {
            Debug.LogError("Received data is invalid or too short.");
            return result;
        }

        // Hàm trợ giúp: Đọc float từ byte[]
        float ReadFloat(byte[] data, ref int offset)
        {
            float value = BitConverter.ToSingle(data, offset);
            offset += 4; // Mỗi float chiếm 4 byte
            return value;
        }

        // Giải mã Body (3 floats cho position + 4 floats cho rotation)
        Vector3 bodyPosition = new Vector3(
            ReadFloat(receivedData, ref offset),
            ReadFloat(receivedData, ref offset),
            ReadFloat(receivedData, ref offset)
        );
        Quaternion bodyRotation = new Quaternion(
            ReadFloat(receivedData, ref offset),
            ReadFloat(receivedData, ref offset),
            ReadFloat(receivedData, ref offset),
            ReadFloat(receivedData, ref offset)
        );

        // Thêm kết quả Body vào danh sách
        result.Add((bodyPosition, bodyRotation));

        int temp = 0;
        // Giải mã từng WheelOut child (3 floats cho position + 4 floats cho rotation mỗi child)
        while (offset + 28 <= receivedData.Length && temp++ < 8) // 28 bytes = 7 floats
        {
            Vector3 childPosition = new Vector3(
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset)
            );
            Quaternion childRotation = new Quaternion(
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset)
            );

            // Thêm vào danh sách
            result.Add((childPosition, childRotation));
        }

        return result;
    }
    public List<Quaternion> DecodeRotateData(byte[] receivedData, ref int offset)
    {
        List<Quaternion> result = new();

        // Kiểm tra dữ liệu nhận được có hợp lệ không
        if (receivedData == null || receivedData.Length < 16)   // 16 byte = 4 float
        {
            Debug.LogError("Received data is invalid or too short.");
            return result;
        }

        // Hàm trợ giúp: Đọc float từ byte[]
        float ReadFloat(byte[] data, ref int offset)
        {
            float value = BitConverter.ToSingle(data, offset);
            offset += 4; // Mỗi float chiếm 4 byte
            return value;
        }


        int temp = 0;
        // Giải mã từng WheelOut child (3 floats cho position + 4 floats cho rotation mỗi child)
        while (offset + 16 <= receivedData.Length && temp++ < 2) // 28 bytes = 7 floats
        {
            Quaternion rotation = new Quaternion(
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset),
                ReadFloat(receivedData, ref offset)
            );

            // Thêm vào danh sách
            result.Add(rotation);
        }

        return result;
    }

    public (float damage, int type, int index) DecodeDamageData(byte[] receivedData)
    {
        if (receivedData == null || receivedData.Length != 12)
            throw new ArgumentException("Invalid data array. Must contain exactly 12 bytes.");

        int offset = 0;

        // Decode damage (float, 4 bytes)
        var damage = DecodeFloatFrom4Bytes(receivedData, offset);
        offset += 4;

        // Decode type (int, 4 bytes)
        var type = DecodeIntFrom4Bytes(receivedData, offset);
        offset += 4;

        // Decode index (int, 4 bytes)
        var index = DecodeIntFrom4Bytes(receivedData, offset);

        return (damage, type, index);
    }

    // Helper method to decode a float from 4 bytes
    private float DecodeFloatFrom4Bytes(byte[] data, int offset)
    {
        if (data == null || data.Length < offset + 4)
            throw new ArgumentException("Invalid data array or offset.");

        // Combine 4 bytes into an int representation
        int intRepresentation =
            (data[offset] << 24) |
            (data[offset + 1] << 16) |
            (data[offset + 2] << 8) |
            data[offset + 3];

        // Convert the int representation back to float
        return BitConverter.ToSingle(BitConverter.GetBytes(intRepresentation), 0);
    }

    // Helper method to decode an int from 4 bytes
    private int DecodeIntFrom4Bytes(byte[] data, int offset)
    {
        if (data == null || data.Length < offset + 4)
            throw new ArgumentException("Invalid data array or offset.");

        // Combine 4 bytes into an int
        return (data[offset] << 24) |
               (data[offset + 1] << 16) |
               (data[offset + 2] << 8) |
               data[offset + 3];
    }
}
