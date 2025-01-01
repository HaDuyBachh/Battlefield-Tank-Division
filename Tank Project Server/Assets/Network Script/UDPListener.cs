using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using static GeneralSystem;
using Debug = UnityEngine.Debug;

public class UDPListener : MonoBehaviour
{
    public int listenPort = 9999; // Cổng nhận từ server.c
    private UdpClient udpClient;
    private NetworkGeneral general;
    private SystemValue system;
    private Process serverProcess; // Để quản lý server chạy bên ngoài
    void Start()
    {
        StartServer();

        system = GetComponent<SystemValue>();
        udpClient = new UdpClient(listenPort);
        Debug.Log($"UDPListener is listening on port {listenPort}");
        BeginReceive();
    }

    public void SetNetWorkGeneral(NetworkGeneral g)
    {
        general = g;
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

            Debug.Log("da nhan data tu unity sender");

            switch (DecodeOnceWithCheckByte(receivedData)[0].command)
            {
                case (byte)Command.Login:
                case (byte)Command.Register:
                case (byte)Command.StartGame:
                case (byte)Command.EndGame:
                    SendResponse(Compress(system.RecvData(receivedData)), remoteEndPoint);
                    break;
                default:
                    ///Nhận câu lệnh:
                    var current_client = general.RecvData(receivedData);

                    // Đã nén gói
                    SendResponse(Compress(general.GetDataRespond(current_client)), remoteEndPoint);
                    //Debug.Log("Phản hồi lại: ");
                    break;
            }
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
    void StartServer()
    {
        string serverPath = Path.Combine(Application.streamingAssetsPath, "server.exe");

        if (File.Exists(serverPath))
        {
            serverProcess = new Process();
            serverProcess.StartInfo.FileName = serverPath;

#if UNITY_EDITOR
            serverProcess.StartInfo.UseShellExecute = true; // Chạy bình thường trong Unity Editor
#else
        serverProcess.StartInfo.UseShellExecute = false; // Đảm bảo tương thích trên build
        serverProcess.StartInfo.CreateNoWindow = false;  // Không mở cửa sổ console
        serverProcess.StartInfo.RedirectStandardOutput = true;
        serverProcess.StartInfo.RedirectStandardError = true;
#endif

            serverProcess.Start();
            Debug.Log("Server started from StreamingAssets.");
        }
        else
        {
            Debug.LogError($"Server executable not found at {serverPath}");
        }
    }
    private void OnApplicationQuit()
    {
        udpClient.Close();

        if (serverProcess != null && !serverProcess.HasExited)
        {
            serverProcess.Kill();
            Debug.Log("Server process terminated.");
        }
    }
}