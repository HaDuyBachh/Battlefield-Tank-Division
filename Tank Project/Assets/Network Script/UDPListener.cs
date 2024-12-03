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
            string message = Encoding.UTF8.GetString(receivedData);
            Debug.Log($"Received from server.c: {message}");

            // Xử lí dữ liệu
            general.SetRevc(message);

            // Gửi phản hồi lại server.c
            SendResponse("lis" + message , remoteEndPoint);
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

    void SendResponse(string message, IPEndPoint remoteEndPoint)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, remoteEndPoint);
        Debug.Log($"Response sent to server.c: {message}");
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
