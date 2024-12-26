using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginSceneControl : MonoBehaviour
{
    public TextMeshProUGUI IP;
    public TextMeshProUGUI mainClient;

    public void SetClickSignIn()
    {
        var sys = FindAnyObjectByType<SystemValue>();
        sys.SetServerIP(IP.text);
        sys.SetClientID(mainClient.text);
    }    
}
