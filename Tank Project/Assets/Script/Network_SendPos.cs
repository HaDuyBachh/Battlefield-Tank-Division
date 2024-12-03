using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_SendPos : MonoBehaviour
{
    public NetworkGeneral general;
    public Transform Body;
    public Transform IdlerWheel;
    public Transform WheelOut;
    public Transform WheelIn;
    public Transform SprocketWheel;
    public void Start()
    {
        general = FindAnyObjectByType<NetworkGeneral>();
    }

    public string GetValue()
    {
        string str = Body.transform.localPosition + "$" + Body.transform.localRotation;
        for (int i = 8; i < WheelOut.childCount; i++)
        {
            str += "$";
            str += WheelOut.GetChild(i).localPosition + "$" + WheelOut.GetChild(i).localRotation;
        }
        return str;
    }
    public void Update()
    {
        general.SetSendStr(GetValue());
    }

}
