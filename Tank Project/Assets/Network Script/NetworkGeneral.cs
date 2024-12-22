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
        Login,
        Move,
        Rotate
    }
    public List<GameObject> clientIdMoveControl;
    public byte[] SendData
    {
        get
        {
            return null;
            // Kiểm tra sự tồn tại của sendStr[1] và sendStr[2]
            if (sendData[1] == null || sendData[2] == null)
            {
                return null; // Trả về null nếu một trong các mảng không tồn tại
            }

            // Sử dụng List<byte> để ghép các mảng byte
            List<byte> byteList = new List<byte>();

            // Thêm sendStr[1] vào List
            byteList.AddRange(sendData[1]);

            // Thêm sendStr[2] vào List
            byteList.AddRange(sendData[2]);

            // Chuyển List<byte> trở lại thành mảng byte
            return byteList.ToArray();
        }
    }
    private List<byte>[] sendData = new List<byte>[100];
    private void Init()
    {
        for (int i = 1; i < clientIdMoveControl.Count; i++)
        {
            clientIdMoveControl[i].GetComponent<Network_Move_Control>().SetID(i);
            clientIdMoveControl[i].GetComponent<NetworkSendMoveData>().SetID(i);
        }
    }
    private void Awake()
    {

    }
    public void SetMoveDataRespond(byte[] dataRespond, int id)
    {
        //this.sendStr[id] = sendStr;
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

            if (command == 0)
            {
                Debug.Log("End Of Command");
                break;
            }

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
                    //SetRevcRotate(DecodeRotateData(data), id);
                    break;

            }

            // Giải mã gói hiện tại
            //Debug.Log($"Decoded Packet:");
            //Debug.Log($"Command: {command}");
            //Debug.Log($"ID: {id}");
            //Debug.Log($"Data Length: {dataLength}");

            // Cập nhật offset để xử lý gói tiếp theo
            offset += 4 + dataLength;
        }

        Debug.Log("Finished decoding all packets.");
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
        // Kiểm tra nếu dữ liệu rỗng hoặc không đủ byte
        if (receivedData == null || receivedData.Length < 2)
        {
            Debug.LogWarning("Received data is too short to decode.");
            return new Vector3[2];
        }

        // Loại bỏ byte đầu tiên và chuyển phần còn lại về chuỗi
        byte[] messageBytes = new byte[receivedData.Length - 1];
        Array.Copy(receivedData, 1, messageBytes, 0, messageBytes.Length);

        // Chuyển đổi byte[] thành chuỗi bất kỳ...

        return new Vector3[2];
    }

    // Set Receive
    public void SetRevcRotate(Vector3[] recvData, int id)
    {
        Debug.Log("Thông số xoay là: " + id + "    " + recvData);
        var control = clientIdMoveControl[id].GetComponent<Network_Rotate_Control>();
        control.SetTarget(recvData[1], recvData[2]);
    }
    public void SetRevcMove(string recvData, int id)
    {
        Debug.Log("Thông số di chuyển là: " + id + "    " + recvData);
        var control = clientIdMoveControl[id].GetComponent<Network_Move_Control>(); 
        control.ResetValue();
        if (recvData.Length > 0)
        {
            string[] cmd = recvData.Split(' ');
            for (int i = 0; i < cmd.Length; i++)
            {
                if (cmd[i].Length == 0) continue;

                switch (cmd[i][0])
                {
                    case 'L':
                        control.Left = true;
                        break;
                    case 'R':
                        control.Right = true;
                        break;
                    case 'F':
                        control.Forward = true;
                        break;
                    case 'B':
                        control.Backward = true;
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
