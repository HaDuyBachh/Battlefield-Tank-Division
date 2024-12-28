using ChobiAssets.PTM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_Rotate_Control : MonoBehaviour
{
    public Aiming_Control_CS aim;

    public void Awake()
    {
        aim = GetComponent<Aiming_Control_CS>();
    }

    public void SetTarget(Vector3 target, Vector3 adj)
    {
        aim.Target_Position = target;
        
        if ( adj != Vector3.zero)
        {
            aim.Adjust_Angle = adj;
            aim.reticleAimingFlag = true;
        }    
    }
}
