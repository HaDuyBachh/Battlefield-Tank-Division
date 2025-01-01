using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralSystem;

public class DashboardSceneControl : MonoBehaviour
{
    public SystemValue sys;
    public UDPSender udp;
    public void Start()
    {
        sys = FindAnyObjectByType<SystemValue>();
        udp = FindAnyObjectByType<UDPSender>();
    }
    public void SendStartRequest()
    {
        List<byte> sendData = new();
        sendData.Add(CheckByte);
        sendData.AddRange(Encode(StringToByte("0000"), (byte)Command.StartGame, (byte)sys.mainClientID));
        udp.SendData(sendData.ToArray());
        Debug.Log("Da gui di");
    }
    public void OnClickStartGame()
    {
        SendStartRequest();
    }    
}
