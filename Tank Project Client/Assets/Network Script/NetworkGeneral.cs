using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGeneral : MonoBehaviour
{
    public List<(Vector3 position, Quaternion rotation)>[] revcStr = new List<(Vector3 position, Quaternion rotation)>[10];

    public void Start()
    {
        revcStr[1] = new();
        revcStr[2] = new();
    }
    public void SetRevc(List<(Vector3 position, Quaternion rotation)> revcStr, int id)
    {
        this.revcStr[id] = revcStr;
    }
}
