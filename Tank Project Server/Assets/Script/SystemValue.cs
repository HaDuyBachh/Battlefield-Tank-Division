using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static GeneralSystem;

public class SystemValue : MonoBehaviour
{
    private bool[] clientId = new bool[1025];
    private DatabaseConnect database;
    private void Initialize()
    {
        database = GetComponent<DatabaseConnect>();
    }    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Initialize();
    }
  
    public byte[] RecvData(byte[] encodedData)
    {
        foreach (var (command, id, dataLength, data) in DecodeWithCheckByte(encodedData))
        {
            switch (command)
            {
                case (byte)Command.Login:
                    return HandleLogin(data);
                case (byte)Command.Register:
                    return HandleRegister(data);
                default:
                    return new byte[0];
            }
        }

        return new byte[0];
    }

    private byte[] HandleRegister(byte[] data)
    {
        return new byte[0];
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


}
