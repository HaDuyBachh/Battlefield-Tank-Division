using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GeneralSystem;

public class LoginSceneControl : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI password;
    public TextMeshProUGUI serverIP;
    public TextMeshProUGUI serverPort;
    public SystemValue sys;
    public UDPSender udp;

    public void Start()
    {
        sys = FindAnyObjectByType<SystemValue>();
        udp = FindAnyObjectByType<UDPSender>();
    }

    public void SendLoginRequest()
    {
        string data = sys.username + "|" + sys.password;
        List<byte> sendData = new();
        sendData.Add(CheckByte);
        sendData.AddRange(Encode(StringToByte(data), (byte)Command.Login, (byte)sys.mainClientID));
        udp.SendData(sendData.ToArray());
    }
    public void SetClickSignIn()
    {
        sys.SetUserName(username.text);
        sys.SetPassword(password.text);

        if (sys.username.Length > 0 && sys.password.Length > 0)
            SendLoginRequest();
    }

    public void SetClickQuit()
    {
        FindAnyObjectByType<SceneControl>().Quit();
    }

    public void SetClickRegister()
    {
        FindAnyObjectByType<SceneControl>().LoadRegisterAfter();
    }

    public void OnClickSetPort()
    {
        sys.SetServerPort(serverPort.text);
    }

    public void OnClickSetIP()
    {
        sys.SetServerIP(serverIP.text);
    }

    public void OnClickQuit()
    {
        FindAnyObjectByType<SceneControl>().Quit();
    }
}
