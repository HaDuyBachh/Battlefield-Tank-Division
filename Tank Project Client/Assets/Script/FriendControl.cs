using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendControl : MonoBehaviour
{
    private bool getFriendList = false;
    private float getFriendListDelta = 0.0f;
    private float getFriendListTimeout = 1.0f;
    [SerializeField]
    private List<RoomObj> roomObjs = new();
    private DashboardSceneControl control;
    private string[] name = new string[0];

    private void Awake()
    {
        control = FindAnyObjectByType<DashboardSceneControl>();
        roomObjs.ForEach(x =>
        {
            x.gameObject.SetActive(false);
        });
    }

    public void ClickFriendPanel()
    {
        getFriendList = true;
        roomObjs.ForEach(x =>
        {
            x.gameObject.SetActive(false);
        });
    }

    public void LeaveFriendPanel()
    {
        getFriendList = false;
    }


    private void Update()
    {
        if (getFriendList)
        {
            getFriendListDelta -= Time.deltaTime;
            if (getFriendListDelta <= 0)
            {
                getFriendListDelta = getFriendListTimeout;
                control.SendGetAllPlayersList();
            }

            if (name.Length > 0)
            {
                Debug.Log("Đang in ra");
                for (int i = 0; i < roomObjs.Count; i++)
                {
                    if (i < name.Length)
                    {
                        roomObjs[i].setDescription(name[i]);
                        roomObjs[i].gameObject.SetActive(true);
                    }
                    else
                        roomObjs[i].gameObject.SetActive(false);
                }

                name = new string[0];
            }
        }
    }

    public void SetNameList(string[] name)
    {
        this.name = name;
    }
}
