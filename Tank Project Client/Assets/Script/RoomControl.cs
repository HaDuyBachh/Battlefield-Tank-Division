using Michsky.UI.Heat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomControl : MonoBehaviour
{
    [SerializeField]
    private int _mode, _num, _time;
    [SerializeField]
    private bool _friendlyfire = false;
    [SerializeField]
    private int currentRoomCode = 0;
    [SerializeField]
    private PanelManager panel;
    [SerializeField]
    private GameObject panelButton;

    [SerializeField]
    private bool getPlayerInRoom = false;
    private float getPlayerInRoomDelta = 0.0f;
    private float getPlayerInRoomTimeout = 1.0f;
    private bool openPlayerInRoomPanel = false;
    private string[] playerName = new string[0];

    [SerializeField]
    private List<GameObject> playerObj;

    private DashboardSceneControl control;
    private void Awake()
    {
        control = GetComponent<DashboardSceneControl>();
    }

    public void ClickGameMode(int mode)
    {
        _mode = mode;
    }
    public void ClickFriendlyFire(bool state)
    {
        _friendlyfire = state;
    }
    public void ClickNumberOfPlayer(int num)
    {
        _num = num;
    }
    public void ClickMatchTime(int time)
    {
        _time = time;
    }
    public void ClickCreateRoom()
    {
        control.SendCreateRoomRequest(_mode, _num, _time);
    }

    public void ClickLeaveRoom()
    {
        if (currentRoomCode > 0) LeaveRoom();
    }

    public void NewRoom()
    {
        _mode = 0;
        _num = 2;
        _time = 1;
        _friendlyfire = false;
        currentRoomCode = 0;

        playerObj.ForEach(x => x.SetActive(false));
        panelButton.SetActive(false);
    }
    public void LeaveRoom()
    {
        currentRoomCode = 0;
        getPlayerInRoom = false;
        openPlayerInRoomPanel = false;
        panelButton.SetActive(true);
    }
    public void Update()
    {
        if (getPlayerInRoom)
        {
            getPlayerInRoomDelta -= Time.deltaTime;
            if (getPlayerInRoomDelta <= 0)
            {
                control.SendGetPlayerInRoom(currentRoomCode);
                getPlayerInRoomDelta = getPlayerInRoomTimeout;  
            }

            if (playerName != null && playerName.Length > 0)
            {
                for (int i = 0; i< playerObj.Count; i++)
                {
                    if (i < playerName.Length)
                    {
                        playerObj[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = playerName[i];
                        if (!playerObj[i].activeSelf) playerObj[i].SetActive(true);
                    }
                    else
                        playerObj[i].SetActive(false);
                }

                playerName = new string[0];
            }

            if (!openPlayerInRoomPanel)
            {
                panel.OpenPanelByIndex(1);
                openPlayerInRoomPanel = true;
            }
        }
    }
    public void SetNewRoom(int code)
    {
        currentRoomCode = code;
        getPlayerInRoom = true;
        getPlayerInRoomDelta = 0.0f;
        openPlayerInRoomPanel = false;
        playerObj.ForEach(x => x.SetActive(false));
    }

    public void SetPlayerName(string[] name)
    {
        playerName = name;
    }

    internal void SetNewRoomCodeAfter()
    {
        throw new NotImplementedException();
    }
}
