using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener: MonoBehaviour
{
    public NetworkSender sender;
    string sendStr;
    public void GetInput()
    {
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
            Debug.Log(sendStr);
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
