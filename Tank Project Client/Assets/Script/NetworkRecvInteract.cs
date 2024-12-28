using ChobiAssets.PTM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralSystem;

public class NetworkRecvInteract : MonoBehaviour
{
    public NetworkSender sender = null;
    public Cannon_Fire_CS cannon = null;
    public Damage_Control_Center_CS damage_Control;
    public NetworkObjectControl control;
    public bool isMain = false;
    private void Awake()
    {
        if (sender == null) sender = FindAnyObjectByType<NetworkSender>();
        if (cannon == null) cannon = GetComponentInChildren<Cannon_Fire_CS>();
        if (control == null) control = GetComponent<NetworkObjectControl>();
        if (damage_Control == null) damage_Control = GetComponent<Damage_Control_Center_CS>();
        cannon.networkInteract = this;
    }
    public void NetworkCallDamage((float damage, int type, int index) value)
    {
        Debug.Log("Damage: " + value.damage + "  " + value.type + "  " + value.index);
        damage_Control.current_Damage = value;
    }
    public void SetMain(bool state)
    {
        if (cannon == null) cannon = GetComponentInChildren<Cannon_Fire_CS>();
        isMain = state;
        cannon.SetEnable(isMain);
    }
    public void SendActiveFire()
    {
        sender.SendInteractData(Command.Fire);
    }
    public void SendActiveChangeFire()
    {
        sender.SendInteractData(Command.ChangeFire);
    }
    public void NetworkCallFire()
    {
        cannon.NetworkCallFire();
    }

    public void NetworkCallChangeFire()
    {
        cannon.NetworkCallChangeFire();
    }
}
