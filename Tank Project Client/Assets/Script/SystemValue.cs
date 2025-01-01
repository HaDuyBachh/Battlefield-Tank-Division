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
                default:
                    break;
            }
        }
    }

    private void HandleStartGame()
    {
        sceneControl.LoadTutorialAfter();
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
}
