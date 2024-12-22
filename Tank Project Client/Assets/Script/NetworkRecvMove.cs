using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRecvMove : MonoBehaviour
{
    public int id;
    public NetworkGeneral general;
    public Transform Body;
    public Transform WheelOut;
    public void Start()
    {
        general = FindAnyObjectByType<NetworkGeneral>();
    }
    public void Thaydoi()
    {

    }    
    public void SetValue()
    {
        if (id == 2) Debug.Log("Đang chạy đây pa");

        int d = 0;
        Body.transform.localPosition = Vector3.Lerp(Body.transform.localPosition, general.revcMoveData[id][d].position, Time.deltaTime * 6f);

        Debug.Log(Body.transform.localPosition + "   " + general.revcMoveData[id][d].position);

        Body.transform.localRotation = Quaternion.Lerp(Body.transform.localRotation, general.revcMoveData[id][d].rotation, Time.deltaTime * 6f);

        for (int i = 8; i < WheelOut.childCount; i++)
        {
            ++d;
            WheelOut.GetChild(i).localPosition = Vector3.Lerp(WheelOut.GetChild(i).localPosition, general.revcMoveData[id][d].position, Time.deltaTime * 6f);
            WheelOut.GetChild(i).localRotation = Quaternion.Lerp(WheelOut.GetChild(i).localRotation, general.revcMoveData[id][d].rotation, Time.deltaTime * 6f);
        }
    }    

    public void Update()
    {
       if (general.revcMoveData[id].Count > 0 ) SetValue();
    }
}
