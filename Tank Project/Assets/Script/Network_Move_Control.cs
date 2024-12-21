using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_Move_Control : MonoBehaviour
{
    public int id = 0;
    public bool Forward = false;
    public bool Backward = false;
    public bool Left = false;
    public bool Right = false;
    public bool Brake = false;

    public void SetID(int id)
    {
        this.id = id;
    }
    public void ResetValue()
    {
        Forward = Backward = Left = Right = Brake = false;
    }    
}
