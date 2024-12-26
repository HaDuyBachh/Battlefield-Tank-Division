using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public int clientID = 1;
    public int clientPort = 8880;
    public int clientQuanty = 6;
    public NetworkSender sender;
    public UDPSender UDP;
    public void ResetClient(int clientPort, byte clientID)
    {
        UDP.SetClientPort(clientPort);
        sender.SetMainID(clientID);
    }
    public void Awake()
    {
        var sys = FindAnyObjectByType<SystemValue>();
        if (sys != null)
        {
            UDP.serverIP = sys.serverIP;
            UDP.serverPort = sys.serverPort;
            clientID = sys.mainClientID;
        }
        ResetClient(clientPort, (byte)clientID);
    }

    ////For test change clientID
    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        ResetClient(clientPort, (byte)clientID);
    //    }    
    //}
}
