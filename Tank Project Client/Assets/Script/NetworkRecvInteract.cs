using ChobiAssets.PTM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralSystem;

public class NetworkRecvInteract : MonoBehaviour
{
    public NetworkSender sender = null;
    public Cannon_Fire_CS cannon = null;
    public NetworkObjectControl control;
    public Damage_Control_04_Track_Collider_CS[] damageTrack;
    public bool isNotMain = true;
    private void Awake()
    {
        if (sender == null) sender = FindAnyObjectByType<NetworkSender>();
        if (cannon == null) cannon = GetComponentInChildren<Cannon_Fire_CS>();
        if (control == null) control = GetComponent<NetworkObjectControl>();
        cannon.networkInteract = this;
    }
    public void SetNotMain(bool state)
    {
        if (cannon == null) cannon = GetComponentInChildren<Cannon_Fire_CS>();
        isNotMain = state;
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
