using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSendMoveData : MonoBehaviour
{
    public int id;
    public NetworkGeneral general;
    public Transform Body;
    public Transform WheelOut;
    public void Start()
    {
        general = FindAnyObjectByType<NetworkGeneral>();
    }
    public void SetID(int id)
    {
        this.id = id;
    }
    public byte[] GetValue()
    {
        // Tạo danh sách chứa các giá trị float từ position và rotation
        List<float> dataList = new();

        // Lấy position và rotation của Body
        Vector3 bodyPosition = Body.transform.localPosition;
        Quaternion bodyRotation = Body.transform.localRotation;

        dataList.Add(bodyPosition.x);
        dataList.Add(bodyPosition.y);
        dataList.Add(bodyPosition.z);

        dataList.Add(bodyRotation.x);
        dataList.Add(bodyRotation.y);
        dataList.Add(bodyRotation.z);
        dataList.Add(bodyRotation.w);

        // Lặp qua các đối tượng con từ WheelOut
        for (int i = 8; i < WheelOut.childCount; i++)
        {
            Transform child = WheelOut.GetChild(i);

            Vector3 childPosition = child.localPosition;
            Quaternion childRotation = child.localRotation;

            dataList.Add(childPosition.x);
            dataList.Add(childPosition.y);
            dataList.Add(childPosition.z);

            dataList.Add(childRotation.x);
            dataList.Add(childRotation.y);
            dataList.Add(childRotation.z);
            dataList.Add(childRotation.w);
        }

        // Chuyển danh sách float thành mảng byte
        List<byte> byteList = new();
        foreach (float value in dataList)
        {
            byte[] floatBytes = BitConverter.GetBytes(value); // Mỗi float thành 4 byte
            byteList.AddRange(floatBytes);
        }

        return byteList.ToArray(); // Trả về mảng byte
    }
    public void Update()
    {
        general.SetMoveDataRespond(GetValue(),id);
    }

}
