using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetworkGeneral : MonoBehaviour
{
    public enum Command
    {
        None,
        Move,
        Rotate,
        Login,
    }
    public List<NetworkObjectControl> clientIdObjectControl;
    private List<byte>[]sendMoveData = new List<byte>[256];
    private List<byte>[]sendRotData = new List<byte>[256];
    private List<byte> respond = new();
    public void Awake()
    {
        for (int id = 1; id < clientIdObjectControl.Count; id++)
        {
            clientIdObjectControl[id].SetID(id);
            sendMoveData[id] = new();
            sendRotData[id] = new();
        }
    }
    public byte[] GetMoveDataRespond()
    {
        respond.Clear();

        /// Chuyển thông điệp thành byte  --------------------------------------

        for (byte id = 1; id < clientIdObjectControl.Count; id++)
        {
            ///[command]:   1 byte
            respond.Add((byte)Command.Move);
            ///[id]:        1 byte
            respond.Add(id);
            ///[length]:    2 byte
            respond.AddRange(EncodeIntTo2Bytes(sendMoveData[id].Count + sendRotData[id].Count));
            /////[data]: length byte
            respond.AddRange(sendMoveData[id]);
            respond.AddRange(sendRotData[id]);
        }

        if (respond.Count > 0) return respond.ToArray();
        else
        {
            respond.Add((byte)Command.None);
            return respond.ToArray();
        }
    }
    public void SetMoveDataRespond(List<byte> data, int id)
    {
        sendMoveData[id] = data;
    }
    public void SetRotateDataRespond(List<byte> data, int id)
    {
        sendRotData[id] = data;
    }
    public void RecvData(byte[] encodedData)
    {
        int offset = 1;

        while (offset < encodedData.Length)
        {
            // Kiểm tra xem có đủ dữ liệu tối thiểu để đọc header (4 byte)
            if (encodedData.Length - offset < 4)
            {
                Debug.Log($"Incomplete data at offset {offset}. Remaining bytes: {encodedData.Length - offset}");
                break;
            }

            // Đọc command và id
            byte command = encodedData[offset];

            byte id = encodedData[offset + 1];

            // Đọc độ dài dữ liệu (2 byte)
            int dataLength = BitConverter.ToInt16(new byte[] { encodedData[offset + 3], encodedData[offset + 2] }, 0);

            // Kiểm tra xem có đủ dữ liệu để đọc toàn bộ gói (header + data)
            if (encodedData.Length - offset < 4 + dataLength)
            {
                Debug.Log($"Incomplete packet at offset {offset}. Expected length: {4 + dataLength}, but got: {encodedData.Length - offset}");
                break;
            }

            // Lấy phần dữ liệu thực tế
            byte[] data = new byte[dataLength];
            Array.Copy(encodedData, offset + 4, data, 0, dataLength);

            switch (command)
            {
                case (byte)Command.Move:
                    SetRevcMove(DecodeMoveData(data), id);
                    break;
                case (byte)Command.Rotate:
                    SetRevcRotate(DecodeRotateData(data), id);
                    break;

            }

            ////Giải mã gói hiện tại
            //Debug.Log($"Decoded Packet:");
            //Debug.Log($"Command: {command}");
            //Debug.Log($"ID: {id}");
            //Debug.Log($"Data Length: {dataLength}");
            //if (command == 1) Debug.Log($"Data Mess: {Encoding.UTF8.GetString(data)}");

            // Cập nhật offset để xử lý gói tiếp theo
            offset += 4 + dataLength;
        }

        //Debug.Log("Finished decoding all packets.");
    }

    // Decode
    public string DecodeMoveData(byte[] receivedData)
    {
        // Kiểm tra nếu dữ liệu rỗng hoặc không đủ byte
        if (receivedData == null || receivedData.Length == 0)
        {
            Debug.LogWarning("Received data is too short to decode.");
            return "";
        }
        // Chuyển đổi byte[] thành chuỗi UTF-8
        string message = Encoding.UTF8.GetString(receivedData);

        return message;
    }
    public Vector3[] DecodeRotateData(byte[] receivedData)
    {
        return ByteArrayToVector3Array(receivedData,2);
    }

    // Set Receive
    public void SetRevcRotate(Vector3[] recvData, int id)
    {
        Debug.Log("Thông số xoay là: " + id + "    " + recvData[0] + "  " + recvData[1]);
        clientIdObjectControl[id].rotate_Control.SetTarget(recvData[0], recvData[1]);
    }
    public void SetRevcMove(string recvData, int id)
    {
        Debug.Log("Thông số di chuyển là:" + id + "   " + recvData);

        clientIdObjectControl[id].move_Control.ResetValue();

        if (recvData.Length > 0)
        {
            string[] cmd = recvData.Split(' ');
            for (int i = 0; i < cmd.Length; i++)
            {
                if (cmd[i].Length == 0) continue;

                switch (cmd[i][0])
                {
                    case 'X':
                        return;
                    case 'L':
                        clientIdObjectControl[id].move_Control.Left = true;
                        break;
                    case 'R':
                        clientIdObjectControl[id].move_Control.Right = true;
                        break;
                    case 'F':
                        clientIdObjectControl[id].move_Control.Forward = true;
                        break;
                    case 'B':
                        clientIdObjectControl[id].move_Control.Backward = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Function Decode
    public byte[] StringToByte(string data)
    {
        return Encoding.UTF8.GetBytes(data);
    }
    public static byte[] Vector3ArrayToByte(Vector3[] v3)
    {
        int curr = 0;
        byte[] byteArray = new byte[v3.Length * 3 * sizeof(float)];
        for (int i = 0; i < v3.Length; i++)
        {
            var en = Vector3ToByte(v3[i]);
            Array.Copy(en, 0, byteArray, curr, en.Length);
            curr += en.Length;
        }

        return byteArray;
    }
    public static Vector3[] ByteArrayToVector3Array(byte[] byteArray, int vectorCount)
    {
        // Kiểm tra xem số lượng byte có hợp lệ không với số lượng Vector3 cần giải mã
        if (byteArray.Length != vectorCount * 3 * sizeof(float))
        {
            throw new ArgumentException("Byte array length does not match the expected length for the given number of Vector3 elements");
        }

        Vector3[] vectorArray = new Vector3[vectorCount];
        int curr = 0;

        for (int i = 0; i < vectorCount; i++)
        {
            // Giải mã mỗi Vector3 từ mảng byte
            byte[] vectorBytes = new byte[3 * sizeof(float)];
            Array.Copy(byteArray, curr, vectorBytes, 0, vectorBytes.Length);
            vectorArray[i] = ByteToVector3(vectorBytes);

            // Cập nhật chỉ số cho lần giải mã tiếp theo
            curr += vectorBytes.Length;
        }

        return vectorArray;
    }
    public static byte[] Vector3ToByte(Vector3 vector)
    {
        // Tạo mảng byte để chứa ba thành phần float của Vector3
        byte[] bytes = new byte[3 * sizeof(float)];

        // Chuyển đổi các thành phần float của Vector3 thành byte
        Array.Copy(BitConverter.GetBytes(vector.x), 0, bytes, 0, sizeof(float));
        Array.Copy(BitConverter.GetBytes(vector.y), 0, bytes, sizeof(float), sizeof(float));
        Array.Copy(BitConverter.GetBytes(vector.z), 0, bytes, 2 * sizeof(float), sizeof(float));

        return bytes;
    }
    public static Vector3 ByteToVector3(byte[] bytes)
    {
        if (bytes.Length != 3 * sizeof(float))
            throw new ArgumentException("Byte array length is not valid for a Vector3");

        // Chuyển đổi các byte thành các giá trị float
        float x = BitConverter.ToSingle(bytes, 0);
        float y = BitConverter.ToSingle(bytes, sizeof(float));
        float z = BitConverter.ToSingle(bytes, 2 * sizeof(float));

        return new Vector3(x, y, z);
    }
    public static byte[] EncodeIntTo2Bytes(int value)
    {
        // Kiểm tra giá trị nằm trong phạm vi của 2 byte
        if (value < -32768 || value > 65535)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between -32768 and 65535 for 2-byte encoding.");

        // Tách byte cao và byte thấp
        byte highByte = (byte)(value >> 8); // Byte cao
        byte lowByte = (byte)(value & 0xFF); // Byte thấp

        return new byte[] { highByte, lowByte };
    }
}
