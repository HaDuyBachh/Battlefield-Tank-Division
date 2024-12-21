using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public int clientID = 1;
    public int clientPort = 8880;
    public int clientQuanty = 2;
    public NetworkSender sender;
    public UDPSender UDP;
    public void Awake()
    {
        UDP.SetClientPort(clientPort);
        sender.SetMainID((byte)clientID);
    }
}
