using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_RecvPos : MonoBehaviour
{
    public int id;
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
    public void SetValue()
    {
        int d = 0;
        Body.transform.localPosition = Vector3.Lerp(Body.transform.localPosition, general.revcStr[id][d].position, Time.deltaTime * 6f);

        Debug.Log(Body.transform.localPosition + "   " + general.revcStr[id][d].position);

        Body.transform.localRotation = Quaternion.Lerp(Body.transform.localRotation, general.revcStr[id][d].rotation, Time.deltaTime * 6f);

        for (int i = 8; i < WheelOut.childCount; i++)
        {
            ++d;
            WheelOut.GetChild(i).localPosition = Vector3.Lerp(WheelOut.GetChild(i).localPosition, general.revcStr[id][d].position, Time.deltaTime * 6f);
            WheelOut.GetChild(i).localRotation = Quaternion.Lerp(WheelOut.GetChild(i).localRotation, general.revcStr[id][d].rotation, Time.deltaTime * 6f);
        }
    }    

    public void Update()
    {
       if (general.revcStr[id].Count > 0 ) SetValue();
    }
}
