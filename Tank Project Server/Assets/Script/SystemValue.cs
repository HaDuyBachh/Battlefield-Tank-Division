using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static GeneralSystem;

public class SystemValue : MonoBehaviour
{
    private bool[] clientId = new bool[1025];
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
        foreach (var (command, id, dataLength, data) in DecodeWithCheckByte(encodedData))
        {
            Debug.Log("Nhan du lieu: command:" + command + "  id:" + id);
            switch (command)
            {
                case (byte)Command.Login:
                    return HandleLogin(data);
                case (byte)Command.Register:
                    return HandleRegister(data);
                case (byte)Command.StartGame:
                    return HandleStartGame();
                case (byte)Command.EndGame:
                    return HandleEndGame();
                case (byte)Command.RemoveClientId:
                    HandleRemoveClientId(id);
                    break;
                default:
                    return new byte[0];
            }
        }

        return new byte[0];
    }

    private void HandleRemoveClientId(int id)
    {
        clientId[id] = false;
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
    private byte[] HandleClientId()
    {
        for (int i = 1; i < clientId.Length; i++)
        {
            if (!clientId[i])
            {
                clientId[i] = true;
                return EncodeIntTo4Bytes(i);
            }
        }
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
            sendData = HandleClientId();
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
