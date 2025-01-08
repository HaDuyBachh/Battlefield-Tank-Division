using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralSystem;

public class DashboardSceneControl : MonoBehaviour
{
    public SystemValue sys;
    public UDPSender udp;
    public RoomControl roomControl;
    public RoomListControl roomListControl;
    public PanelManager panelManager;
    public int changePanel = -1;
    public int roomcode = 0;

    public void Awake()
    {
        sys = FindAnyObjectByType<SystemValue>();
        udp = FindAnyObjectByType<UDPSender>();
        roomControl = GetComponent<RoomControl>();
        roomListControl = GetComponent<RoomListControl>();
        sys.dashboard = this;
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

    public void SendJoinRoom(int code)
    {
        List<byte> sendData = new();
        sendData.Add(CheckByte);
        sendData.AddRange(Encode(EncodeIntTo4Bytes(code), (byte)Command.JoinRoom, (byte)sys.mainClientID));
        udp.SendData(sendData.ToArray());
    }

    public void SendGetRoomList()
    {
        List<byte> sendData = new();
        sendData.Add(CheckByte);
        sendData.AddRange(Encode(EncodeIntTo4Bytes(0), (byte)Command.GetRoomList, (byte)sys.mainClientID));
        udp.SendData(sendData.ToArray());
    }

    public void SceneChange(int id)
    {
        if (id == 1) roomControl.NewRoom(); else roomControl.LeaveRoom();
        if (id == 2) roomListControl.ClickListRoom(); else roomListControl.LeaveListRoom();
    }

    public void Update()
    {
        if (changePanel > -1)
        {
            panelManager.OpenPanelByIndex(changePanel);
            roomControl.SetNewRoom(roomcode);
            changePanel = -1;
        }    
    }
    public void SetJoinRoom(int code)
    {   
        changePanel = 1;
        roomcode = code;
    }    

}