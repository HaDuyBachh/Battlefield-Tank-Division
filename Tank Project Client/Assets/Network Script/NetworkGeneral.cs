using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGeneral : MonoBehaviour
{
    private string revcStr = "";
    public string RevcStr { get { return revcStr; } }
    public void SetRevc(string revcStr)
    {
        this.revcStr = revcStr;
    }
}
