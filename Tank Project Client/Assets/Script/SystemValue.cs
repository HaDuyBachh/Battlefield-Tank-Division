using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static GeneralSystem;

public class SystemValue : MonoBehaviour
{
    public string serverIP = "127.0.0.1"; // IP của server.c
    public int serverPort = 8080;        // Cổng của server.c
    public int mainClientID = -1;
    public string username = "";
    public string password = "";
    public SceneControl sceneControl;
    public DashboardSceneControl dashboard;

    public void Awake()
    {
        if (FindObjectsOfType<SystemValue>().Length > 1) Destroy(this.gameObject);
        DontDestroyOnLoad(this);
        sceneControl = GetComponent<SceneControl>();
    }
    public void SetServerIP(string ip)
    {
        string s = "";
        foreach (var c in ip)
        {
            if (c == '.' || ('0' <= c && c <= '9')) s += c;
        }
        if (s.Length != 0) serverIP = s;

        GetComponent<UDPSender>().SetServerIP(serverIP);
    }
    public void SetServerPort(string serverPort)
    {
        string s = "";
        foreach (var c in serverPort)
        {
            if (('0' <= c && c <= '9')) s += c;
            else break;
        }
        this.serverPort = int.Parse(s);
    }
    public void SetUserName(string username)
    {
        this.username = "";
        foreach (var c in username)
        {
            if (!ValidChar(c)) return;
            this.username += c;
        }
    }
    public void SetPassword(string password)
    {
        this.password = "";

        foreach (var c in password)
        {
            if (!ValidChar(c)) return;
            this.password += c;
        }
    }
    public string GetStringValid(string str)
    {
        var s = "";
        foreach (var c in str)
        {
            if (!ValidChar(c)) return s;
            s += c;
        }

        return s;
    }
    public void RecvData(byte[] encodedData)
    {
        foreach (var (command, id, dataLength, data) in DecodeWithoutCheckByte(encodedData))
        {
            Debug.Log("Sys- Command:" + command + " id: " + id);
            switch ((Command)command)
            {
                case Command.Login:
                    HandleLogin(data);
                    break;
                case Command.Register:
                    HandleRegister(data);
                    break;
                case Command.StartGame:
                    HandleStartGame();
                    break;
                case Command.CreateRoom:
                    HandleCreateRoom(data);
                    break;
                case Command.GetPlayerInRoom:
                    HandleGetPlayerInRoom(data);
                    break;
                case Command.GetRoomList:
                    HandleGetRoomList(data);
                    break;
                case Command.JoinRoom:
                    HandleJoinRoom(data);
                    break;
                case Command.GetAllPlayersList:
                    HandleGetAllPlayerList(data);
                    break;
                default:
                    break;
            }
        }
    }

    private void HandleGetAllPlayerList(byte[] data)
    {
        var playerString = Encoding.UTF8.GetString(data);
        string[] playerNames = playerString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var n in playerNames)
        {
            Debug.Log("Ten nguoi chơi la: " + n);
        }

        dashboard.friendControl.SetNameList(playerNames);
    }

    private void HandleJoinRoom(byte[] data)
    {
        Debug.Log("Dữ liệu room coce nhận được là: " + data.Length);
        int roomcode = Decode4BytesToInt(data);
        Debug.Log(roomcode);

        if (roomcode == 0)
        {
            Debug.LogError("Error room code");
            return;
        }

        dashboard.SetJoinRoom(roomcode);
    }    

    private void HandleGetRoomList(byte[] data)
    {
        Debug.Log("Đã nhận được: " + data.Length + "dữ liệu");

        if (data.Length % 16 != 0)
        {
            Debug.LogError("Invalid data length: cannot parse room list.");
            return;
        }

        var rooms = new List<Room>();
        var temp = new byte[4];
        int st = 0;
        while (st < data.Length)
        {
            Room r = new();
            Array.Copy(data, st, temp, 0, 4);
            r.code = Decode4BytesToInt(temp);

            st += 4;
            Array.Copy(data, st, temp, 0, 4);
            r.mode = Decode4BytesToInt(temp);

            st += 4;
            Array.Copy(data, st, temp, 0, 4);
            r.number = Decode4BytesToInt(temp);

            st += 4;
            Array.Copy(data, st, temp, 0, 4);
            r.time = Decode4BytesToInt(temp);

            st += 4;

            rooms.Add(r);
        }

        dashboard.roomListControl.SetRoomList(rooms.ToArray());
    }

    private void HandleGetPlayerInRoom(byte[] data)
    {
        var playerString = Encoding.UTF8.GetString(data);
        string[] playerNames = playerString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var n in playerNames)
        {
            Debug.Log("Ten nguoi chơi la: " + n);
        }

        dashboard.roomControl.SetPlayerName(playerNames);
    }

    private void HandleCreateRoom(byte[] data)
    {
        Debug.Log("Dữ liệu room coce nhận được là: " + data.Length);
        int roomcode = Decode4BytesToInt(data);
        Debug.Log(roomcode);

        if (roomcode == 0)
        {
            Debug.LogError("Error room code");
            return;
        }
        dashboard.roomControl.SetNewRoom(roomcode);
    }

    Coroutine _startGame = null;
    private void HandleStartGame()
    {
       sceneControl.LoadTutorialAfter(1.0f);
    } 

    private void HandleRegister(byte[] data)
    {
        if (Encoding.UTF8.GetString(data).Contains("SUCCESS"))
        {
            Debug.Log("Dang ky thanh cong");
            sceneControl.LoadSignInAfter();
        }
        else
            Debug.Log("Dang ky that bai");
    }

    public void HandleLogin(byte[] data)
    {
        int ID = Decode4BytesToInt(data);
        if (ID > 0)
        {
            mainClientID = ID;
            sceneControl.LoadDashboardAfter();
        }
    }

    public byte[] RemoveClientId()
    {
        var data = new List<byte>();
        data.Add(CheckByte);
        data.AddRange(Encode(StringToByte(""), (byte)Command.RemoveClientId, (byte)mainClientID));

        return data.ToArray();
    }    


    public void OnApplicationQuit()
    {
        if (mainClientID > 0) GetComponent<UDPSender>().SendData(RemoveClientId());
    }
}
