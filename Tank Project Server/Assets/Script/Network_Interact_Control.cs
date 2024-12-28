using ChobiAssets.PTM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_Interact_Control : MonoBehaviour
{
    public Cannon_Fire_CS cannon;
    private void Awake()
    {
        cannon = cannon.GetComponentInChildren<Cannon_Fire_CS>();
    }
    public bool Fire()
    {
        return cannon.NetworkFire();
    }    
    public bool ChangeFire()
    {
        return cannon.NetworkChangeFire();
    }    
}
