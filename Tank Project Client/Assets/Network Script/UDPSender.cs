using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPSender : MonoBehaviour
{
    public NetworkGeneral general;
    private UdpClient udpClient;
    public string serverIP = "127.0.0.1"; // IP của server.c
    public int serverPort = 8080;        // Cổng của server.c
    public int clientPort = 8888;       // Cổng để nhận phản hồi từ server.c

    void Start()
    {
        general = FindObjectOfType<NetworkGeneral>();
        udpClient = new UdpClient(clientPort); // Lắng nghe phản hồi trên cổng clientPort
        Debug.Log("UDPSender started.");

        // Gửi dữ liệu đến server.c
        SendData("Hello from Unity!");

        // Lắng nghe phản hồi
        BeginReceive();
    }

    public void SendData(string message)
    {
        byte CHECK_BYTE = 0x11; // Giá trị cần để vượt qua kiểm tra trong server.c

        // Chuyển thông điệp thành byte
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        // Tạo mảng dữ liệu mới với CHECK_BYTE ở đầu
        byte[] data = new byte[messageBytes.Length + 1];
        data[0] = CHECK_BYTE; // Gán CHECK_BYTE vào byte đầu tiên
        Array.Copy(messageBytes, 0, data, 1, messageBytes.Length); // Sao chép thông điệp vào sau CHECK_BYTE

        // Gửi dữ liệu qua UDP
        udpClient.Send(data, data.Length, serverIP, serverPort);
        Debug.Log($"Sent to server.c: CHECK_BYTE={CHECK_BYTE}, Message={message}");
    }


    public void BeginReceive()
    {
        udpClient.BeginReceive(OnReceive, null);
    }

    public void OnReceive(IAsyncResult ar)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receivedData = udpClient.EndReceive(ar, ref remoteEndPoint);
        general.SetRevc(DecodeTransformData(receivedData));
        Debug.Log($"Received from server.c");
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
    }

    public List<(Vector3 position, Quaternion rotation)> DecodeTransformData(byte[] receivedData)
    {
        List<(Vector3 position, Quaternion rotation)> result = new List<(Vector3 position, Quaternion rotation)>();

        // Kiểm tra dữ liệu nhận được có hợp lệ không
        if (receivedData == null || receivedData.Length < 28)
        {
            Debug.LogError("Received data is invalid or too short.");
            return result;
        }

        int offset = 0;

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

        // Giải mã từng WheelOut child (3 floats cho position + 4 floats cho rotation mỗi child)
        while (offset + 28 <= receivedData.Length) // 28 bytes = 7 floats
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
