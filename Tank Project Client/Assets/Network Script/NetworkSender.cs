using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static GeneralSystem;

public class NetworkSender : MonoBehaviour
{
    [SerializeField]
    private byte mainID = 0;
    [SerializeField]
    private UDPSender udp;
    private float nextSendITimeout = 0.005f; // Interval in seconds between each send
    private float nextSendTime = 0f;

    // All Data can send to server -----------------------------------------------

    //Chuỗi gửi đi
    List<byte> sendData = new();

    //Chuỗi di chuyển
    private string moveData = "";

    //Gồm 2 vị trí để xoay xe tăng (1: Target Position Network, 2: Adjust Angle Network)
    private Vector3[] rotateData = new Vector3[2];
    private bool rotateChange = false;
    public void SetMainID(byte id)
    {
        /// Id chính của main
        mainID = id;

        /// Cấp phát ID phụ
        int id_temp = 1;
        foreach (var control in FindObjectsOfType<NetworkObjectControl>())
        {
            if (control.gameObject.CompareTag("MainPlayer"))
            {
                control.SetID(mainID);
            }
            else
            {
                id_temp += (id_temp != mainID) ? 0 : 1;
                control.SetID(id_temp++);
            }
        }
    }
    public void Start()
    {
        udp = FindAnyObjectByType<UDPSender>();
    }
    public void Update()
    {
        nextSendTime -= Time.deltaTime;
        if (nextSendTime <= 0.0f)
        {
            SendMoveData();
        }
    }
    public void SendMoveData()
    {
        sendData.Add(0x11);

        ///Encode Move
        if (moveData.Length > 0)
        {
            sendData.AddRange(Encode(StringToByte(moveData), (byte)Command.Move, mainID));
            moveData = "";
        }
        
        ///Encode Rotate
        if (rotateChange)
        {
            sendData.AddRange(Encode(Vector3ArrayToByte(rotateData), (byte)Command.Rotate, mainID));
            rotateChange = false;
        }

        if (sendData.Count < 2) sendData.AddRange(Encode(StringToByte("X "), (byte)Command.Move, mainID));

        Debug.Log("Đang chạy ở đây với send là: " + sendData.Count); 

        udp.SendData(sendData.ToArray());
            

        //End Of Send
        sendData.Clear();
        nextSendTime = nextSendITimeout;

        udp.BeginReceive();
    }
    public void SendInteractData(Command type)
    {
        var data = new List<byte> { 0x11 };
        data.AddRange(Encode(new byte[0], (byte)type, mainID));
        udp.SendData(data.ToArray());
        udp.BeginReceive();
    }
    public void SendInteractiveData()
    {

    }
    public void SetMoveData(string moveData)
    {
        this.moveData = moveData;
    }
    public void SetRotateData(Vector3 target_Position, Vector3 adjust_Angle)
    {
        rotateData[0] = target_Position;
        rotateData[1] = adjust_Angle;
        rotateChange = true;
    }    
}
