using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRecvRot : MonoBehaviour
{
    public NetworkObjectControl control;
    public Transform turret;
    public Transform cannon;

    public void SetValue()
    {
        int d = 0;
        turret.localRotation = 
            Quaternion.Lerp(turret.localRotation, control.general.revcRotData[control.ID][d++], Time.deltaTime * 6f);
        cannon.localRotation =
            Quaternion.Lerp(cannon.localRotation, control.general.revcRotData[control.ID][d++], Time.deltaTime * 6f);
    }
}
