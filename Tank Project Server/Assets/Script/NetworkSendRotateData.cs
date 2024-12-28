using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSendRotateData : MonoBehaviour
{
    public Transform turret;
    public Transform cannon;
    
    public List<byte> GetValue()
    {
        List<float> dataList = new();

        Quaternion turretRotation = turret.transform.localRotation;

        dataList.Add(turretRotation.x);
        dataList.Add(turretRotation.y);
        dataList.Add(turretRotation.z);
        dataList.Add(turretRotation.w);

        Quaternion cannonRotation = cannon.transform.localRotation;

        dataList.Add(cannonRotation.x);
        dataList.Add(cannonRotation.y);
        dataList.Add(cannonRotation.z);
        dataList.Add(cannonRotation.w);

        List<byte> byteList = new();
        foreach (float value in dataList)
        {
            byte[] floatBytes = BitConverter.GetBytes(value); // Mỗi float thành 4 byte
            byteList.AddRange(floatBytes);
        }

        return byteList;
    }
}
