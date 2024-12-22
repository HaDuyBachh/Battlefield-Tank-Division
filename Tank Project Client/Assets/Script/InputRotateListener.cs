using ChobiAssets.PTM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRotateListener : MonoBehaviour
{
    public NetworkSender sender;
    public Aiming_Control_CS aim;
    public void GetInput()
    {
        if (aim.cameraRotationScript.Horizontal_Input != 0 
            || aim.cameraRotationScript.Vertical_Input!= 0)
                sender.SetRotateData(aim.Target_Position, aim.Adjust_Angle);
    }
    public void Start()
    {
        sender = FindAnyObjectByType<NetworkSender>();
        aim = GetComponent<Aiming_Control_CS>();
    }
    public void Update()
    {
        GetInput();
    }
}
