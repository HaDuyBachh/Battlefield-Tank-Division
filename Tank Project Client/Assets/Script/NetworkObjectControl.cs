using ChobiAssets.PTM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectControl : MonoBehaviour
{
    [SerializeField]
    private int id = 0;
    public NetworkGeneral general;
    private NetworkRecvMove recvMoveData;
    private NetworkRecvRot recvRotateData;
    private NetworkRecvInteract recvInteract;
    private InputMoveListener inputMove;
    private InputRotateListener inputRotate;

    public void Awake()
    {
        //init
        if (general == null) general = FindAnyObjectByType<NetworkGeneral>();
        if (recvMoveData == null)  recvMoveData = GetComponent<NetworkRecvMove>();
        if (recvRotateData == null)  recvRotateData = GetComponent<NetworkRecvRot>();
        if (recvInteract == null)  recvInteract = GetComponent<NetworkRecvInteract>();

        if (inputMove == null) inputMove = GetComponent<InputMoveListener>();
        if (inputRotate == null) inputRotate = GetComponent<InputRotateListener>();
        recvMoveData.control = this;
        recvRotateData.control = this;
    }

    public void SetMain(bool state)
    {
        if (recvInteract == null) recvInteract = GetComponent<NetworkRecvInteract>();
        recvInteract.SetMain(state);

        foreach (var damage in GetComponents<Damage_Control_00_Base_CS>())
        {
            if (damage is not Damage_Control_04_Track_Collider_CS) damage.enabled = false;
        }
        foreach (var damage in GetComponentsInChildren<Damage_Control_00_Base_CS>())
        {
            if (damage is not Damage_Control_04_Track_Collider_CS) damage.enabled = false;
        }
    }

    public int ID { get { return id; } }
    public void SetID(int id)
    {
        this.id = id;

        if (general == null) general = FindAnyObjectByType<NetworkGeneral>();
        general.AddRecvInteract(GetComponent<NetworkRecvInteract>(), id); 
    }
    public void Update()
    {
        if (general.revcMoveData[id].Count > 0) recvMoveData.SetValue();
        if (general.revcRotData[id].Count > 0) recvRotateData.SetValue();
        if (general.revcFireData[id])
        {
            //Debug.Log("ID: " + id + " Đã gọi bắn")
            general.revcFireData[id] = false;
            recvInteract.NetworkCallFire();
        }
    }
}
