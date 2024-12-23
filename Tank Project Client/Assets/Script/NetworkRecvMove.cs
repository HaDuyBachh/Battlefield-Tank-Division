using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRecvMove : MonoBehaviour
{
    public NetworkObjectControl control;
    public Transform Body;
    public Transform WheelOut;

    public void SetValue()
    {
        int d = 0;
        Body.transform.localPosition = Vector3.Lerp(Body.transform.localPosition, control.general.revcMoveData[control.ID][d].position, Time.deltaTime * 6f);

        //Debug.Log(Body.transform.localPosition + "   " + control.general.revcMoveData[control.ID][d].position);

        Body.transform.localRotation = Quaternion.Lerp(Body.transform.localRotation, control.general.revcMoveData[control.ID][d].rotation, Time.deltaTime * 6f);

        for (int i = 8; i < WheelOut.childCount; i++)
        {
            ++d;
            WheelOut.GetChild(i).localPosition = Vector3.Lerp(WheelOut.GetChild(i).localPosition, control.general.revcMoveData[control.ID][d].position, Time.deltaTime * 6f);
            WheelOut.GetChild(i).localRotation = Quaternion.Lerp(WheelOut.GetChild(i).localRotation, control.general.revcMoveData[control.ID][d].rotation, Time.deltaTime * 6f);
        }
    }    
}
