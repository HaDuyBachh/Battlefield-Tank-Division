using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBasic : MonoBehaviour
{
    string sendStr = "";
    void Update()
    {
        sendStr = "";
        if (Input.GetKey(KeyCode.DownArrow))
        {
            sendStr += "B ";
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            sendStr += "F ";
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            sendStr += "L ";
        }    
        if (Input.GetKey(KeyCode.RightArrow))
        {
            sendStr += "R ";
        }    
        if (sendStr.Length > 0)
        {
            Debug.Log(sendStr);
        }    
    }
}
