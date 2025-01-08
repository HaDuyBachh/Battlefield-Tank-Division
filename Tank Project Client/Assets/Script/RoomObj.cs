using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RoomObj : MonoBehaviour
{
    public TextMeshProUGUI Description;
    public int code = 0;
    public RoomListControl control;

    public void setDescription(string txt)
    {
        Description.text = txt;
    }    

    public void OnClick()
    {
        control.ClickRoomObject(code);
    }    
}
