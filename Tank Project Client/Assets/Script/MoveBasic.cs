using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBasic : MonoBehaviour
{
    public bool forward;
    public bool backward;
    public bool left;
    public bool right;
    public void setFalse()
    {
        forward = backward = left = right = false;
    }    
    public void L() =>left = true;
    public void R() => right = true;
    public void F() => forward = true;
    public void B() => backward = true;

    void Awake()
    {
        setFalse();
    }

    void Update()
    {
        if (forward)
        {
            transform.position += transform.forward;
        }

        setFalse();
    }
}
