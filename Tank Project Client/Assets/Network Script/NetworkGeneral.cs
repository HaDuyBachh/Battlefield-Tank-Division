using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGeneral : MonoBehaviour
{
    private List<(Vector3 position, Quaternion rotation)> revcStr = new();
    public List<(Vector3 position, Quaternion rotation)> RevcStr { get { return revcStr; } }
    public void SetRevc(List<(Vector3 position, Quaternion rotation)> revcStr)
    {
        this.revcStr = revcStr;
    }
}
