using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGeneral : MonoBehaviour
{
    public ClientManager clientManager;
    public List<(Vector3 position, Quaternion rotation)>[] revcMoveData;

    public void Init()
    {
        ///Khởi tạo revcStr
        clientManager = FindAnyObjectByType<ClientManager>();
        revcMoveData = new List<(Vector3 position, Quaternion rotation)>[clientManager.clientQuanty+1];
        for (int i = 1; i < revcMoveData.Length; i++)
        {
            revcMoveData[i] = new();
        }    

        ///Khởi tạo
    }    
    public void Awake()
    {
        Init();
    }
    public void SetRevc(byte[] receivedData)
    {
        int offset = 0;
        SetMoveRevc(DecodeTransformData(receivedData, ref offset), 1);
        Debug.Log("Dữ liệu off là: " + offset);
        SetMoveRevc(DecodeTransformData(receivedData, ref offset), 2);
        Debug.Log("Dữ liệu off 2 là: " + offset);
    }    

    public void SetMoveRevc(List<(Vector3 position, Quaternion rotation)> revcStr, int id)
    {
        revcMoveData[id] = revcStr;
    }
    public void SetRotRevc()
    {

    }

    public List<(Vector3 position, Quaternion rotation)> DecodeTransformData(byte[] receivedData, ref int offset)
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
}
