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
    }
}
