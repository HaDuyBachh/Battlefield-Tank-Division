using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_RecvPos : MonoBehaviour
{
    public NetworkGeneral general;
    public Transform Body;
    public Transform IdlerWheel;
    public Transform WheelOut;
    public Transform WheelIn;
    public Transform SprocketWheel;

    struct Location
    {
        public Vector3 Pos;
        public Quaternion Rot;
    };

    Location[] aimList = new Location[50];

    public void Start()
    {
        general = FindAnyObjectByType<NetworkGeneral>();
        aimList[0].Pos = (Body.transform.localPosition);
        aimList[1].Rot = (Body.transform.localRotation);
    }

    public Vector3 Pos(string vectorString)
    {
        vectorString = vectorString.Trim('(', ')'); // Loại bỏ dấu ngoặc
        string[] parts = vectorString.Split(',');

        return new Vector3(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2])
        );
    }    

    public Quaternion Rot(string vectorString)
    {
        vectorString = vectorString.Trim('(', ')');

        string[] parts = vectorString.Split(',');

        if (parts.Length != 4)
        {
            Debug.LogError("Invalid input format for Quaternion.");
            return Quaternion.identity; // Trả về giá trị mặc định nếu chuỗi không hợp lệ
        }

        // Chuyển đổi các giá trị chuỗi thành float
        float x = float.Parse(parts[0]);
        float y = float.Parse(parts[1]);
        float z = float.Parse(parts[2]);
        float w = float.Parse(parts[3]);

        // Tạo Quaternion từ các giá trị float
        return new Quaternion(x, y, z, w);
    }    

    public void GetValue(string recvPos)
    {
        string[] value = recvPos.Split("$");
        for (int i = 0; i < value.Length-1; i+=2)
        {
            aimList[i/2].Pos = Pos(value[i]);
            aimList[i/2].Rot = Rot(value[i+1]);
        }    
    }    
    public void SetValue()
    {
        int d = 0;
        Body.transform.localPosition = Vector3.Lerp(Body.transform.localPosition, aimList[d].Pos, Time.deltaTime * 6f);
        Body.transform.localRotation = Quaternion.Lerp(Body.transform.localRotation, aimList[d].Rot, Time.deltaTime * 6f);
        for (int i = 8; i < WheelOut.childCount; i++)
        {
            ++d;
            WheelOut.GetChild(i).localPosition = Vector3.Lerp(WheelOut.GetChild(i).localPosition, aimList[d].Pos, Time.deltaTime * 6f);
            WheelOut.GetChild(i).localRotation = Quaternion.Lerp(WheelOut.GetChild(i).localRotation, aimList[d].Rot, Time.deltaTime * 6f);
        }
    }    

    public void Update()
    {
        if (general.RevcStr.Length > 0) GetValue(general.RevcStr);
        SetValue();
    }
}
