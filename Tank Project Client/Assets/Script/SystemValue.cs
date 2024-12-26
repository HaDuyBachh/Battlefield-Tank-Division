using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemValue : MonoBehaviour
{
    public string serverIP = "127.0.0.1"; // IP của server.c
    public int serverPort = 8080;        // Cổng của server.c
    public int mainClientID;
    public void Awake()
    {
        DontDestroyOnLoad(this);
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
        mainClientID = 0;
        foreach (var m in ID)
        {
            if ('0' <= m && m <= '9') mainClientID = mainClientID * 10 + (m - '0');
        }    
    }

    public void SetServerPort(string serverPort)
    {
        this.serverPort = int.Parse(serverPort);
    }
}
