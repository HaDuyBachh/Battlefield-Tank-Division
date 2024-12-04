using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPListener : MonoBehaviour
{
    public int listenPort = 9999; // Cổng nhận từ server.c
    private UdpClient udpClient;
    private NetworkGeneral general;

    void Start()
    {
        general = FindAnyObjectByType<NetworkGeneral>();
        udpClient = new UdpClient(listenPort);
        Debug.Log($"UDPListener is listening on port {listenPort}");
        BeginReceive();
    }

    void BeginReceive()
    {
        udpClient.BeginReceive(OnReceive, null);
    }

    void OnReceive(IAsyncResult ar)
    {
        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpClient.EndReceive(ar, ref remoteEndPoint);
            string message = DecodeData(receivedData);
            Debug.Log($"Received from server.c: {message}");

            // Xử lí dữ liệu
            general.SetRevcStr(message);

            // Gửi phản hồi lại server.c
            SendResponse(general.SendStr , remoteEndPoint);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving data: {e.Message}");
        }
        finally
        {
            udpClient.BeginReceive(OnReceive, null);
        }
    }

    void SendResponse(byte[] data, IPEndPoint remoteEndPoint)
    {
        udpClient.Send(data, data.Length, remoteEndPoint);
        Debug.Log($"Response sent to server.c");
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
    }
    public string DecodeData(byte[] receivedData)
    {
        // Kiểm tra nếu dữ liệu rỗng hoặc không đủ byte
        if (receivedData == null || receivedData.Length < 2)
        {
            Debug.LogWarning("Received data is too short to decode.");
            return "";
        }
        
        // Loại bỏ byte đầu tiên và chuyển phần còn lại về chuỗi
        byte[] messageBytes = new byte[receivedData.Length - 1];
        Array.Copy(receivedData, 1, messageBytes, 0, messageBytes.Length);

        // Chuyển đổi byte[] thành chuỗi UTF-8
        string message = Encoding.UTF8.GetString(messageBytes);

        return message;
    }

}
