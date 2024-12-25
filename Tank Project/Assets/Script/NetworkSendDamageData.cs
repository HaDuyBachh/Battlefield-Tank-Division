using ChobiAssets.PTM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralSystem;

public class NetworkSendDamageData : MonoBehaviour
{
    public bool hasValue = false;
    public float damage = -1;
    public int type;
    public int index;
    public void SetDamageValue(float damage, int type, int index)
    {
        this.damage = damage;
        this.type = type;
        this.index = index;
        hasValue = true;
    }    
    public byte[] GetValue()
    {
        List<byte> respond = new();
        respond.AddRange(EncodeFloatTo4Bytes(damage));
        respond.AddRange(EncodeIntTo4Bytes(type));
        respond.AddRange(EncodeIntTo4Bytes(index));

        hasValue = false;
        return respond.ToArray();
    }
}
