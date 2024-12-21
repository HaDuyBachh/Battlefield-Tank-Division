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
    public int clientPort = 8880;       // Cổng để nhận phản hồi từ server.c

    public enum Command
    {
        None,
        Move,
        Rotate,
        Login,
    }

    public void SetClientPort(int port)
    {
        clientPort = port;
    }

    void Start()
    {
        general = FindObjectOfType<NetworkGeneral>();
        udpClient = new UdpClient(clientPort); // Lắng nghe phản hồi trên cổng clientPort
        Debug.Log("UDPSender started.");
    }

    public void SendMove(string message,byte command, byte id)
    {
        byte CHECK_BYTE = 0x11; // Giá trị cần để vượt qua kiểm tra trong server.c

        // Chuyển thông điệp thành byte
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        // Tạo mảng dữ liệu mới với CHECK_BYTE ở đầu
        byte[] data = new byte[messageBytes.Length + 3];
        data[0] = CHECK_BYTE; // Gán CHECK_BYTE vào byte đầu tiên
        data[1] = command;
        data[2] = id;
        Array.Copy(messageBytes, 0, data, 3, messageBytes.Length);

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

        int offset = 0;
        general.SetRevc(DecodeTransformData(receivedData, ref offset),1);
        Debug.Log("Dữ liệu off là: " + offset);
        general.SetRevc(DecodeTransformData(receivedData, ref offset),2);
        Debug.Log("Dữ liệu off 2 là: " + offset);
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
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
        while (offset + 28 <= receivedData.Length && temp++<8) // 28 bytes = 7 floats
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
