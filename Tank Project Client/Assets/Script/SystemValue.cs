using System;
using System.Collections;
using System.Collections.Generic;
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
    public void Awake()
    {
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
    }
    public void SetClientID(string ID)
    {
        var temp_mainClientID = 0;
        foreach (var m in ID)
        {
            if ('0' <= m && m <= '9') temp_mainClientID = temp_mainClientID * 10 + (m - '0');
        }

        if (temp_mainClientID > 0) mainClientID = temp_mainClientID;
    }
    public void SetServerPort(string serverPort)
    {
        this.serverPort = int.Parse(serverPort);
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
                default:
                    break;
            }
        }
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
}
