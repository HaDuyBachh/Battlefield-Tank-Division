using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMoveListener: MonoBehaviour
{
    public NetworkSender sender;
    private string sendStr;
    private bool isSent = false;
    public void GetInput()
    {
        sendStr = "";
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            sendStr += "B ";
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            sendStr += "F ";
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            sendStr += "L ";
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            sendStr += "R ";
        }
        if (sendStr.Length > 0)
        {
            isSent = true;
            Debug.Log(sendStr);
            sender.SetMoveData(sendStr);
        }
        else
        if (isSent)
        {
            sendStr = "X ";
            isSent = false;
            sender.SetMoveData(sendStr);
        } 
            
    }

    public void Start()
    {
        sender = FindAnyObjectByType<NetworkSender>();
    }
    public void Update()
    {
        GetInput();
    }
}
