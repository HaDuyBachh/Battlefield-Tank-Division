using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using static GeneralSystem;

public class UDPSender : MonoBehaviour
{
    public NetworkGeneral general;
    public SystemValue system;
    private UdpClient udpClient;
    public string serverIP = "127.0.0.1"; // IP của server.c
    public int serverPort = 8080;        // Cổng của server.c
    public int clientPort = 8880;       // Cổng để nhận phản hồi từ server.c
    public void SetClientPort(int port)
    {
        clientPort = port;
    }
    public void SetNetWorkGeneral(NetworkGeneral general)
    {
        this.general = general;
    }    
    void Start()
    {
        system = GetComponent<SystemValue>();
        udpClient = new UdpClient(clientPort); // Lắng nghe phản hồi trên cổng clientPort
        Debug.Log("UDPSender started.");
    }
    public void SendData(byte[] data)
    {
        // Gửi dữ liệu qua UDP
        udpClient.Send(data, data.Length, serverIP, serverPort);
        //Debug.Log($"Sent to server.c: Message={data.Length}");

        udpClient.BeginReceive(OnReceive, null);
    }

    public void OnReceive(IAsyncResult ar)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        ///Đã giải nén gói
        byte[] receivedData =  Decompress(udpClient.EndReceive(ar, ref remoteEndPoint));

        switch (DecodeOnceWithoutCheckByte(receivedData)[0].command)
        {
            case (byte)Command.Register:
            case (byte)Command.Login:
                system.RecvData(receivedData);
                break;
            default:
                general.RecvData(receivedData);
                break;
        }    
        //Debug.Log("Nhận phản hồi từ server: " + receivedData.Length); 
    }
    private void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
