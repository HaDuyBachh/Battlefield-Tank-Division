using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPSender : MonoBehaviour
{
    private UdpClient udpClient;
    public string serverIP = "127.0.0.1"; // IP của server.c
    public int serverPort = 8080;        // Cổng của server.c
    public int clientPort = 8888;       // Cổng để nhận phản hồi từ server.c

    void Start()
    {
        udpClient = new UdpClient(clientPort); // Lắng nghe phản hồi trên cổng clientPort
        Debug.Log("UDPSender started.");

        // Gửi dữ liệu đến server.c
        SendData("Hello from Unity!");

        // Lắng nghe phản hồi
        BeginReceive();
    }

    public void SendData(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, serverIP, serverPort);
        Debug.Log($"Sent to server.c: {message}");
    }

    public void BeginReceive()
    {
        udpClient.BeginReceive(OnReceive, null);
    }

    public void OnReceive(IAsyncResult ar)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receivedData = udpClient.EndReceive(ar, ref remoteEndPoint);
        string message = Encoding.UTF8.GetString(receivedData);
        Debug.Log($"Received from server.c: {message}");
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
