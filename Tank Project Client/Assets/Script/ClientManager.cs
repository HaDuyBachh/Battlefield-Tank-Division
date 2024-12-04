using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public int clientID = 1;
    public int clientPort = 8880;
    public NetworkSender sender;
    public Network_RecvPos mainPos;
    public Network_RecvPos clonePos;
    public UDPSender UDP;
    public void Awake()
    {
        UDP.clientPort = clientPort;
        sender.id = (byte)clientID;
        mainPos.id = clientID;
        for (int i = 1; i<=2; i++)
        {
            if (i != clientID) clonePos.id = i;
        }
    }
}
