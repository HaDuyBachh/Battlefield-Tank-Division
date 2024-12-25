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
        general = FindAnyObjectByType<NetworkGeneral>();
        recvMoveData = GetComponent<NetworkRecvMove>();
        recvRotateData = GetComponent<NetworkRecvRot>();
        recvInteract = GetComponent<NetworkRecvInteract>();
        
        inputMove = GetComponent<InputMoveListener>();
        inputRotate = GetComponent<InputRotateListener>();
        recvMoveData.control = this;
        recvRotateData.control = this;

        if (!CompareTag("MainPlayer")) SetNotMain();
    }

    public void SetNotMain()
    {
        recvInteract.SetNotMain(true);
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
        general.AddRecvInteract(GetComponent<NetworkRecvInteract>());
         
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
