using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSender : MonoBehaviour
{
    string sendStr = "";
    public UDPSender udp;
    private float sendITimeout = 0.005f; // Interval in seconds between each send
    private float nextSendTime = 0f;

    public void Awake()
    {
        udp = FindAnyObjectByType<UDPSender>();
        Application.targetFrameRate = 60;
    }
    public void SetData(string sendStr)
    {
        this.sendStr = "^" + sendStr;
    }
    public void SendData()
    {
        udp.SendData(sendStr);
        udp.BeginReceive();
        sendStr = "^";
    }    
    public void Update()
    {
        nextSendTime -= Time.deltaTime;
        if (nextSendTime <= 0.0f)
        {
            SendData();
            nextSendTime = sendITimeout;  // Set next send time
        }
    }
}
