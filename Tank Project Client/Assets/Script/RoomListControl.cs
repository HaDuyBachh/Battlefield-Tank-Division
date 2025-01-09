using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralSystem;

public class RoomListControl : MonoBehaviour
{
    [SerializeField]
    private List<RoomObj> roomObjs = new();
    private DashboardSceneControl control;
    private Room[] rooms = new Room[0];

    private bool getRoomList = false;
    private float getRoomListDelta = 0.0f;
    private float getRoomListTimeout = 1.0f;
    private void Awake()
    {
        control = FindAnyObjectByType<DashboardSceneControl>();
        roomObjs.ForEach(x =>
        {
            x.gameObject.SetActive(false);
            x.control = this;
        });
    }

    public void ClickListRoom()
    {
        getRoomList = true;
        roomObjs.ForEach(x =>
        {
            x.gameObject.SetActive(false);
        });
    }    

    public void LeaveListRoom()
    {
        getRoomList = false;
    }    

    private void Update()
    {
        if (getRoomList)
        {
            getRoomListDelta -= Time.deltaTime;
            if (getRoomListDelta <= 0)
            {
                getRoomListDelta = getRoomListTimeout;
                control.SendGetRoomList();
            } 

            if (rooms.Length > 0)
            {
                Debug.Log("Đang in ra");
                for (int i = 0; i<roomObjs.Count; i++)
                {
                    if (i < rooms.Length)
                    {
                        roomObjs[i].setDescription("Mode: " + (rooms[i].mode == 0 ? "Free" : "Conquest") + "\n" + "Code: " + rooms[i].code + "\n" + "Players: " + rooms[i].number + "\n" + "Time: " + rooms[i].time);
                        roomObjs[i].gameObject.SetActive(true);
                        roomObjs[i].code = rooms[i].code;
                    }    
                    else
                        roomObjs[i].gameObject.SetActive(false);
                }

                rooms = new Room[0];
            }    
        }    
    }

    public void SetRoomList(Room[] rooms)
    {
        this.rooms = rooms;
    }    

    public void ClickRoomObject(int code)
    {
        control.SendJoinRoom(code);
    }    
}
