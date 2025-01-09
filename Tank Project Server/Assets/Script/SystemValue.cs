using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using static GeneralSystem;

public class SystemValue : MonoBehaviour
{
    [SerializeField]
    private int sessionRoom = 1;
    private Request[] request = new Request[1025];
    private string[] clientId = new string[1025];
    private List<Room> rooms = new();
    private DatabaseConnect database;
    private SceneControl sceneControl;
    private void Initialize()
    {
        database = GetComponent<DatabaseConnect>();
        sceneControl = GetComponent<SceneControl>();
    }
    private void Awake()
    {
        if (FindObjectsOfType<SystemValue>().Length > 1) Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        Initialize();
    }

    public byte[] RecvData(byte[] encodedData)
    {
        List<byte> sendData = new();
        foreach (var (command, id, dataLength, data) in DecodeWithCheckByte(encodedData))
        {
            Debug.Log("Nhan du lieu: command:" + command + "  id:" + id);
            switch (command)
            {
                case (byte)Command.Login:
                    sendData.AddRange(HandleLogin(data));
                    break;
                case (byte)Command.Register:
                    sendData.AddRange(HandleRegister(data));
                    break;
                case (byte)Command.CreateRoom:
                    sendData.AddRange(HandleCreateRoom(data, id));
                    break;
                case (byte)Command.JoinRoom:
                    sendData.AddRange(HandleJoinRoom(data, id));
                    break;
                case (byte)Command.LeaveRoom:
                    sendData.AddRange(HandleLeaveRoom(data, id));
                    break;
                case (byte)Command.GetPlayerInRoom:
                    sendData.AddRange(HandleGetPlayerInRoom(data));
                    break;
                case (byte)Command.GetRoomList:
                    sendData.AddRange(HandleGetRoomList());
                    break;
                case (byte)Command.StartGame:
                    sendData.AddRange(HandleStartGame());
                    break;
                case (byte)Command.GetAllPlayersList:
                    sendData.AddRange(HandleGetAllPlayerList());
                    break;
                case (byte)Command.EndGame:
                    sendData.AddRange(HandleEndGame());
                    break;
                case (byte)Command.RemoveClientId:
                    HandleRemoveClientId(id);
                    break;
            }
        }

        return sendData.ToArray();
    }

    private byte[] HandleGetAllPlayerList()
    {
        string player_name = "";
        foreach (var name in clientId) player_name += name + ';';   
        return Encode(StringToByte(player_name), (byte)Command.GetAllPlayersList, 0).ToArray();
    }

    /// <returns>
    /// dữ liệu trả về một loạt các room:   <br></br>
    /// [int] code                          <br></br>
    /// [int] mode                          <br></br>
    /// [int] num
    /// [int] time
    /// </returns>
    private byte[] HandleGetRoomList()
    {
        List<byte> sendData = new();
        foreach (var r in rooms)
        {
            sendData.AddRange(EncodeIntTo4Bytes(r.code));
            sendData.AddRange(EncodeIntTo4Bytes(r.mode));
            sendData.AddRange(EncodeIntTo4Bytes(r.number));
            sendData.AddRange(EncodeIntTo4Bytes(r.time));
        }

        return Encode(sendData.ToArray(), (byte)Command.GetRoomList, 0).ToArray();
    }

    /// <summary>
    /// dữ liệu data: <br></br>
    /// [int] room code 
    /// </summary>
    /// <returns>
    /// dữ liệu:  <br></br>
    /// [string] chuỗi player (cách nhau bởi ';')
    /// </returns>
    private byte[] HandleGetPlayerInRoom(byte[] data)
    {
        var code = Decode4BytesToInt(data);
        string player_name = "";
        foreach (var r in rooms)
        {
            if (r.code == code)
            {
                foreach (var id in r.Player)
                {
                    player_name += clientId[id] + ';';  
                }
                break;
            }
        }

        return Encode(StringToByte(player_name), (byte)Command.GetPlayerInRoom, 0).ToArray();
    }

    /// <summary>
    /// dữ liệu data: <br></br>
    /// [int] room code 
    /// </summary>
    private byte[] HandleLeaveRoom(byte[] data, int playerID)
    {
        var code = Decode4BytesToInt(data);
        byte[] sendData = new byte[0];
        foreach (var r in rooms)
        {
            if (r.code == code)
            {
                if (r.Player.Contains(playerID))
                {
                    r.Player.Remove(playerID);
                    if (r.Player.Count == 0)
                    {
                        rooms.Remove(r);
                    }

                    sendData = StringToByte("OK");
                }
                else
                {
                    sendData = StringToByte("NOT");
                }
                break;
            }
        }

        return Encode(sendData, (byte)Command.LeaveRoom, (byte)playerID).ToArray();
    }

    /// <summary>
    /// dữ liệu data: <br></br>
    /// [int] room code 
    /// </summary>
    private byte[] HandleJoinRoom(byte[] data, int playerID)
    {
        var code = Decode4BytesToInt(data);
        byte[] sendData = new byte[0];
        foreach (var r in rooms)
        {
            if (r.code == code)
            {
                if (r.Player.Count < r.number)// && !r.Player.Contains(playerID))
                {
                    r.Player.Add(playerID);
                    sendData = EncodeIntTo4Bytes(code);
                    Debug.Log("Trả về room ở đây nè: " + code);
                }
                else
                {
                    sendData = EncodeIntTo4Bytes(0);
                    Debug.Log("không trả về");
                }
                break;
            }
        }

        return Encode(sendData, (byte)Command.JoinRoom, (byte)playerID).ToArray();
    }

    /// <summary>
    /// [int] mode     <br></br>
    /// [int] number   <br></br>
    /// [int] time     <br></br>
    /// </summary>
    /// <returns>room code</returns>
    private byte[] HandleCreateRoom(byte[] data, int playerID)
    {
        var _data = data.ToList();
        var r = new Room();
        r.mode = Decode4BytesToInt(_data.GetRange(0, 4).ToArray());
        r.number = Decode4BytesToInt(_data.GetRange(4, 4).ToArray());
        r.time = Decode4BytesToInt(_data.GetRange(8, 4).ToArray());

        r.Player = new();
        r.Player.Add(playerID);
        r.code = (++sessionRoom);
        rooms.Add(r);

        Debug.Log("Trả về phòng với dữ liệu là: " + r.code);

        return Encode(EncodeIntTo4Bytes(r.code), (byte)Command.CreateRoom, (byte)playerID).ToArray();
    }

    private void HandleRemoveClientId(int id)
    {
        clientId[id] = null;
        request[id].add_friend_request.Clear();
        request[id].invite_request = 0;
    }
    private byte[] HandleRegister(byte[] data)
    {
        var inputData = Encoding.UTF8.GetString(data);

        var input = inputData.Split('|');
        byte[] sendData = new byte[0];

        if (database.Register(input[0], input[1]).Result.success)
        {
            Debug.Log("Dang ky thanh cong");
            sendData = StringToByte("SUCCESS");
        }
        else
        {
            Debug.Log("Dang ky khong thanh cong");
            sendData = StringToByte("FAIL");
        }


        return Encode(sendData, (byte)Command.Register, 0).ToArray();
    }
    private byte[] HandleClientId(string name)
    {
        for (int i = 1; i < clientId.Length; i++)
        {
            if (clientId[i] == null || clientId[i].Length == 0)
            {
                clientId[i] = name;
                Debug.Log(i + "  " + name);
                return EncodeIntTo4Bytes(i);
            }
        }

        Debug.Log("Khong tim thay client ID thoa man");
        return EncodeIntTo4Bytes(-1);
    }
    public byte[] HandleLogin(byte[] data)
    {
        var inputData = Encoding.UTF8.GetString(data);

        var input = inputData.Split('|');
        byte[] sendData = new byte[0];

        if (database.Login(input[0], input[1]).Result)
        {
            Debug.Log("Dang nhap thanh cong");
            sendData = HandleClientId(input[0]);
        }
        else
        {
            Debug.Log("Dang nhap khong thanh cong");
            sendData = EncodeIntTo2Bytes(-1);
        }

        return Encode(sendData, (byte)Command.Login, 0).ToArray();
    }
    public byte[] HandleStartGame()
    {
        if (!sceneControl.currentScene.Contains("Tutorial"))
        {
            Debug.Log("da nhan duoc start");
            Debug.Log("Scene:: " + sceneControl.currentScene);
            sceneControl.LoadTutorialdAfter();
        }
        else
        {
            Debug.Log("Scene hien tai la: " + sceneControl.currentScene);
        }

        return Encode(new byte[0], (byte)Command.StartGame, 0).ToArray();
    }
    public byte[] HandleEndGame()
    {
        FindAnyObjectByType<SceneControl>().LoadMainAfter();

        return Encode(new byte[0], (byte)Command.EndGame, 0).ToArray();
    }
}
