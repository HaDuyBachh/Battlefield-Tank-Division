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
    public void SendCreateRoomRequest(int mode, int num, int time)
    {
        List<byte> data = new();
        data.AddRange(EncodeIntTo4Bytes(mode));
        data.AddRange(EncodeIntTo4Bytes(num));
        data.AddRange(EncodeIntTo4Bytes(time));

        List<byte> sendData = new();
        sendData.Add(CheckByte);
        sendData.AddRange(Encode(data.ToArray(), (byte)Command.CreateRoom, (byte)sys.mainClientID));
        udp.SendData(sendData.ToArray());
    } 

    public void SendGetPlayerInRoom(int code)
    {
        List<byte> sendData = new();
        sendData.Add(CheckByte);
        sendData.AddRange(Encode(EncodeIntTo4Bytes(code), (byte)Command.GetPlayerInRoom, (byte)sys.mainClientID));
        udp.SendData(sendData.ToArray());
    }    
}