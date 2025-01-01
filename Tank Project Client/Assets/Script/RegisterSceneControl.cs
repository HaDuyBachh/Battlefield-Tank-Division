using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GeneralSystem;

public class RegisterSceneControl : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI password;
    public TextMeshProUGUI correct_password;
    public SystemValue sys;
    public UDPSender udp;

    public void Start()
    {
        sys = FindAnyObjectByType<SystemValue>();
        udp = FindAnyObjectByType<UDPSender>();
    }

    public void SendRegisterRequest(string username, string password)
    {
        Debug.Log("Da gui dang ky di");
        string data = username + "|" + password;
        List<byte> sendData = new();
        sendData.Add(CheckByte);
        sendData.AddRange(Encode(StringToByte(data), (byte)Command.Register, (byte)sys.mainClientID));
        udp.SendData(sendData.ToArray());
    }

    public void Cancel()
    {
        FindAnyObjectByType<SceneControl>().LoadSignInAfter();
    }

    public void SetClickRegister()
    {
        var usr = sys.GetStringValid(username.text);
        var psw = sys.GetStringValid(password.text);
        var correct_psw = sys.GetStringValid(correct_password.text);

        if (usr.Length > 0 && psw.Length > 0 && correct_psw == psw)
            SendRegisterRequest(usr, psw);
    }
}
