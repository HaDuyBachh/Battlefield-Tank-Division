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
    public void SendData(byte[] data)
    {
        // Gửi dữ liệu qua UDP
        udpClient.Send(data, data.Length, serverIP, serverPort);
        Debug.Log($"Sent to server.c: Message={data.Length}");
    }
    public void BeginReceive()
    {
        udpClient.BeginReceive(OnReceive, null);
    }
    public void OnReceive(IAsyncResult ar)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receivedData = udpClient.EndReceive(ar, ref remoteEndPoint);

        Debug.Log("Nhận phản hồi từ server");
    }
    private void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
