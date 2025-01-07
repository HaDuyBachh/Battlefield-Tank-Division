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
    private const int MAX_PORT_ATTEMPTS = 100;
    public void SetNetWorkGeneral(NetworkGeneral general)
    {
        this.general = general;
    }

    public void SetServerIP(string IP)
    {
        serverIP = IP;
    }
    public void SetClientPort(int port)
    {
        if (IsPortAvailable(port))
        {
            clientPort = port;
            InitializeUdpClient();
        }
        else
        {
            Debug.LogWarning($"Port {port} đã được sử dụng. Tìm port khác...");
            int newPort = FindAvailablePort(port);
            clientPort = newPort;
            InitializeUdpClient();
        }
    }
    private bool IsPortAvailable(int port)
    {
        try
        {
            using (var client = new UdpClient(port))
            {
                client.Close();
                return true;
            }
        }
        catch (SocketException)
        {
            return false;
        }
    }
    private int FindAvailablePort(int startPort)
    {
        int port = startPort;
        int attempts = 0;

        while (attempts < MAX_PORT_ATTEMPTS)
        {
            try
            {
                using (var client = new UdpClient(port))
                {
                    client.Close();
                    Debug.Log($"Tìm thấy port khả dụng: {port}");
                    return port;
                }
            }
            catch (SocketException)
            {
                port++;
                attempts++;
            }
        }
        throw new Exception("Không tìm được port khả dụng sau " + MAX_PORT_ATTEMPTS + " lần thử");
    }
    private void InitializeUdpClient()
    {
        try
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
            udpClient = new UdpClient(clientPort);
            Debug.Log($"UDPSender khởi tạo thành công trên port {clientPort}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Lỗi khởi tạo UDPClient: {ex.Message}");
        }
    }

    void Start()
    {
        SetClientPort(clientPort);
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
            case (byte)Command.StartGame:
            case (byte)Command.EndGame:
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
