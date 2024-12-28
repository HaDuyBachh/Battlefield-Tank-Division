using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectControl : MonoBehaviour
{
    [SerializeField]
    private int id = 0;
    private NetworkGeneral general;
    private NetworkSendMoveData sendMoveData;
    private NetworkSendRotateData sendRotateData;
    private NetworkSendDamageData sendDamageData;
    public Network_Move_Control move_Control;
    public Network_Rotate_Control rotate_Control;
    public Network_Interact_Control interact_Control;
    public void Awake()
    {
        //init
        general = FindAnyObjectByType<NetworkGeneral>();
        sendMoveData = GetComponent<NetworkSendMoveData>();
        sendRotateData = GetComponent<NetworkSendRotateData>();
        sendDamageData = GetComponent<NetworkSendDamageData>();
        move_Control = GetComponent<Network_Move_Control>();
        rotate_Control = GetComponent<Network_Rotate_Control>();
        interact_Control = GetComponent<Network_Interact_Control>();
    }
    public int ID { get { return id; } }
    public void SetID(int id)
    {
        this.id = id;
    }

    public void Update()
    {
        general.SetMoveDataRespond(sendMoveData.GetValue(), ID);
        general.SetRotateDataRespond(sendRotateData.GetValue(), ID);

        if (sendDamageData.hasValue)
            general.SetDamageDataRespond(sendDamageData.GetValue(), ID);
    }
}
