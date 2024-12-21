using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSender : MonoBehaviour
{
    [SerializeField]
    private byte mainID = 0;
    [SerializeField]
    private UDPSender udp;
    private float sendITimeout = 0.005f; // Interval in seconds between each send
    private float nextSendTime = 0f;
    string moveData = "";
    List<byte> rotateData;

    public void SetMainID(byte id)
    {
        this.mainID = id;
    }
    public void InitClientID()
    {
        int id_temp = 1;
        foreach (var body in FindObjectsOfType<Network_RecvPos>())
        {
            if (body.gameObject.CompareTag("MainPlayer"))
            {
                body.id = this.mainID;
            }
            else
            {
                id_temp += (id_temp != mainID) ? 0 : 1;
                body.id = id_temp;
                id_temp++;
            }
        }
    }
    public void Start()
    {
        udp = FindAnyObjectByType<UDPSender>();
        InitClientID();
    }
    public void SetMoveData(string sendStr)
    {
        moveData = sendStr;
    }
    public void SendData()
    {
        //udp.SendMove(sendStr,(byte)UDPSender.Command.Move, mainID);
        //udp.BeginReceive();
        //sendStr = "";
    }    
    public void SetRotateData(Vector3 rotate)
    {
        rotateData = Vector3ToBytes(rotate);
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

    public byte[] Vector3ToBytes(Vector3 vector)
    {
        byte[] bytes = new byte[sizeof(float) * 3];

        // Chuyển đổi các giá trị float thành byte[]
        BitConverter.GetBytes(vector.x).CopyTo(bytes, 0);
        BitConverter.GetBytes(vector.y).CopyTo(bytes, sizeof(float));
        BitConverter.GetBytes(vector.z).CopyTo(bytes, sizeof(float) * 2);

        return bytes;
    }
}
