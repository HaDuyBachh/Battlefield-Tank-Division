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

            ///Nhận câu lệnh:
            general.RecvData(receivedData);

            // Gửi phản hồi lại server.c
            //SendResponse(new byte[] { 0x11, 0x22, 0x12 }, remoteEndPoint);
            SendResponse(general.GetMoveDataRespond(), remoteEndPoint);
            Debug.Log("Phản hồi lại: " + general.GetMoveDataRespond().Length);
            
        }
        finally
        {
            udpClient.BeginReceive(OnReceive, null);
        }
    }
    void SendResponse(byte[] data, IPEndPoint remoteEndPoint)
    {
        udpClient.Send(data, data.Length, remoteEndPoint);
        //Debug.Log($"Response sent to server.c");
    }
    private void OnApplicationQuit()
    {
        udpClient.Close();
    }
}