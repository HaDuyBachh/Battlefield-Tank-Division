using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetworkSender : MonoBehaviour
{
    [SerializeField]
    private byte mainID = 0;
    [SerializeField]
    private UDPSender udp;
    private float nextSendITimeout = 0.005f; // Interval in seconds between each send
    private float nextSendTime = 0f;

    // All Data can send to server -----------------------------------------------

    //Chuỗi gửi đi
    List<byte> sendData = new();

    //Chuỗi di chuyển
    private string moveData = "";

    //Gồm 2 vị trí để xoay xe tăng (1: Target Position Network, 2: Adjust Angle Network)
    private Vector3[] rotateData = new Vector3[2];
    private bool rotateChange = false;

    public enum Command
    {
        None,
        Move,
        Rotate,
        Login,
    }

    public void SetMainID(byte id)
    {
        this.mainID = id;
    }
    public void InitClientID()
    {
        int id_temp = 1;
        foreach (var body in FindObjectsOfType<NetworkRecvMove>())
        {
            if (body.gameObject.CompareTag("MainPlayer"))
            {
                body.id = this.mainID;
            }
            else
            {
                id_temp += (id_temp != mainID) ? 0 : 1;
                body.id = id_temp;
                id_temp++;
            }
        }
    }
    public void Start()
    {
        udp = FindAnyObjectByType<UDPSender>();
        InitClientID();
    }
    public void Update()
    {
        nextSendTime -= Time.deltaTime;
        if (nextSendTime <= 0.0f)
        {
            SendData();
        }
    }
    public void SendData()
    {
        sendData.Add(0x11);

        ///Encode Move
        if (moveData.Length > 0)
        {
            sendData.AddRange(Encode(StringToByte(moveData), (byte)Command.Move, mainID));
            moveData = "";
        }
        
        ///Encode Rotate
        if (rotateChange)
        {
            sendData.AddRange(Encode(Vector3ArrayToByte(rotateData), (byte)Command.Rotate, mainID));
            rotateChange = false;
        }

        Debug.Log("Đang chạy ở đây với send là: " + sendData.Count);

        //DecodeAll(sendData.ToArray());

        udp.SendData(sendData.ToArray());

        //End Of Send
        sendData.Clear();
        nextSendTime = nextSendITimeout;

        udp.BeginReceive();
    }
    public void SetMoveData(string moveData)
    {
        this.moveData = moveData;
    }
    public void SetRotateData(Vector3 target_Position, Vector3 adjust_Angle)
    {
        rotateData[0] = target_Position;
        rotateData[1] = adjust_Angle;
        rotateChange = true;
    }    
    public List<byte> Encode(byte[] messageBytes, byte command, byte id)
    {
        List<byte> data = new List<byte>();

        // Chuyển thông điệp thành byte  --------------------------------------
        ///[command]:   1 byte
        data.Add (command);
        ///[id]:        1 byte
        data.Add(id);
        ///[length]:    2 byte
        data.AddRange(EncodeIntTo2Bytes(messageBytes.Length));
        ///[data]: length byte
        data.AddRange(messageBytes);
       
        return data;
    }
    public static void Decode(byte[] encodedData)
    {
        if (encodedData.Length < 4)
        {
            Debug.Log("Invalid data.");
            return;
        }

        // Đọc command và id
        byte command = encodedData[0];
        byte id = encodedData[1];

        // Đọc length (2 byte) để xác định độ dài của dữ liệu
        int dataLength = BitConverter.ToInt16(new byte[] { encodedData[3], encodedData[2] }, 0);

        // Kiểm tra độ dài dữ liệu
        if (encodedData.Length < 4 + dataLength)
        {
            Debug.Log("Invalid data length: " + encodedData.Length + " < " + dataLength);
            return;
        }

        // Lấy phần dữ liệu thực tế
        byte[] data = new byte[dataLength];
        Array.Copy(encodedData, 4, data, 0, dataLength);

        // In ra các thông tin giải mã
        Debug.Log($"Command: {command}");
        Debug.Log($"ID: {id}");
        Debug.Log($"Data Length: {dataLength}");
        Debug.Log($"Decoded Data: {Encoding.UTF8.GetString(data)}");
    }
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
    public static void DecodeAll(byte[] encodedData)
    {
        int offset = 0;

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

            // Giải mã gói hiện tại
            Debug.Log($"Decoded Packet:");
            Debug.Log($"Command: {command}");
            Debug.Log($"ID: {id}");
            Debug.Log($"Data Length: {dataLength}");

            // Cập nhật offset để xử lý gói tiếp theo
            offset += 4 + dataLength;
        }

        Debug.Log("Finished decoding all packets.");
    }

}
