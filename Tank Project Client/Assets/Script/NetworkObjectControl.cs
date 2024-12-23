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
    private InputMoveListener inputMove;
    private InputRotateListener inputRotate;

    public void Awake()
    {
        //init
        general = FindAnyObjectByType<NetworkGeneral>();
        recvMoveData = GetComponent<NetworkRecvMove>();
        recvRotateData = GetComponent<NetworkRecvRot>();
        inputMove = GetComponent<InputMoveListener>();
        inputRotate = GetComponent<InputRotateListener>();

        recvMoveData.control = this;
        recvRotateData.control = this;
    }

    public int ID { get { return id; } }
    public void SetID(int id)
    {
        this.id = id;
    }
    public void Update()
    {
        if (general.revcMoveData[id].Count > 0) recvMoveData.SetValue();
        if (general.revcRotData[id].Count > 0) recvRotateData.SetValue();
    }
}
