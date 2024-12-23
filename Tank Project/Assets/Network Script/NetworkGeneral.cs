using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static GeneralSystem;

public class NetworkGeneral : MonoBehaviour
{
    public List<NetworkObjectControl> clientIdObjectControl;
    private List<byte>[] sendMoveData = new List<byte>[256];
    private List<byte>[] sendRotData = new List<byte>[256];
    private bool[] fireData = new bool[256];
    private bool[] changeFireData = new bool[256];
    enum Active
    {
        Move,
        Fire,
        ChangeFire
    }
    private bool[] activeList = new bool[Enum.GetValues(typeof(Active)).Length];

    public void Awake()
    {
        for (int id = 1; id < clientIdObjectControl.Count; id++)
        {
            clientIdObjectControl[id].SetID(id);
            sendMoveData[id] = new();
            sendRotData[id] = new();
        }
    }


    //Reveice
    public void RecvData(byte[] encodedData)
    {
        foreach (var (command, id, dataLength, data) in DecodeWithCheckByte(encodedData))
        {
            switch (command)
            {
                case (byte)Command.Move:
                    SetRevcMove(DecodeMoveData(data), id);
                    activeList[(int)Active.Move] = true;
                    break;
                case (byte)Command.Rotate:
                    SetRevcRotate(DecodeRotateData(data), id);
                    activeList[(int)Active.Move] = true;
                    break;
                case (byte)Command.Fire:
                    activeList[(int)Active.Fire] = true;
                    fireData[id] = true;
                    ///Chỗ này sẽ trả về respond luôn vì UDP sẽ theo cơ chế nhận rồi gửi
                    break;
                case (byte)Command.ChangeFire:
                    activeList[(int)Active.ChangeFire] = true;
                    changeFireData[id] = true;
                    ///Chỗ này sẽ trả về respond luôn vì UDP sẽ theo cơ chế nhận rồi gửi
                    break;
            }
        }
    }

    //Get Data ResPond
    public byte[] GetDataRespond()
    {
        List<byte> respond = new(); 
        for (int i = 0; i< Enum.GetValues(typeof(Active)).Length; i++)
        {
            if (activeList[i])
            {
                switch ((Active)i)
                {
                    case Active.Move:
                        respond.AddRange(GetMoveDataRespond());
                        break;
                    case Active.Fire:
                        respond.AddRange(GetFireInteractDataRespond());
                        break;
                    case Active.ChangeFire:
                        respond.AddRange(GetChangeFireInteractDataRespond());
                        break;
                }

                activeList[i] = false;
            }
        }
        return respond.ToArray();
    }
    public List<byte> GetMoveDataRespond()
    {
        List<byte> respond = new();

        /// Chuyển thông điệp thành byte  --------------------------------------

        for (byte id = 1; id < clientIdObjectControl.Count; id++)
        {
            ///[command]:   1 byte
            respond.Add((byte)Command.Move);
            ///[id]:        1 byte
            respond.Add(id);
            ///[length]:    2 byte
            respond.AddRange(EncodeIntTo2Bytes(sendMoveData[id].Count + sendRotData[id].Count));
            /////[data]: length byte
            respond.AddRange(sendMoveData[id]);
            respond.AddRange(sendRotData[id]);
        }

        if (respond.Count > 0)
            return respond;
        else
        {
            respond.Add((byte)Command.None);
            return respond;
        }
    }
    public List<byte> GetFireInteractDataRespond()
    {
        List<byte> respond = new();

        for (byte id = 1; id < clientIdObjectControl.Count; id++)
        {
            if (fireData[id])
            {
                fireData[id] = false;
                ///[command]:   1 byte
                respond.Add((byte)Command.Fire);
                ///[id]:        1 byte
                respond.Add(id);
                ///[length]:    2 byte
                respond.AddRange(EncodeIntTo2Bytes(0));
            }
        }

        return respond;
    }
    public List<byte> GetChangeFireInteractDataRespond()
    {
        List<byte> respond = new();

        for (byte id = 1; id < clientIdObjectControl.Count; id++)
        {
            if (changeFireData[id])
            {
                changeFireData[id] = false;
                ///[command]:   1 byte
                respond.Add((byte)Command.ChangeFire);
                ///[id]:        1 byte
                respond.Add(id);
                ///[length]:    2 byte
                respond.AddRange(EncodeIntTo2Bytes(0));
            }
        }

        return respond;
    }



    //Set Data Respond
    public void SetMoveDataRespond(List<byte> data, int id)
    {
        sendMoveData[id] = data;
    }
    public void SetRotateDataRespond(List<byte> data, int id)
    {
        sendRotData[id] = data;
    }

    // Decode
    public string DecodeMoveData(byte[] receivedData)
    {
        // Kiểm tra nếu dữ liệu rỗng hoặc không đủ byte
        if (receivedData == null || receivedData.Length == 0)
        {
            Debug.LogWarning("Received data is too short to decode.");
            return "";
        }
        // Chuyển đổi byte[] thành chuỗi UTF-8
        string message = Encoding.UTF8.GetString(receivedData);

        return message;
    }
    public Vector3[] DecodeRotateData(byte[] receivedData)
    {
        return ByteArrayToVector3Array(receivedData, 2);
    }

    // Set Receive
    public void SetRevcRotate(Vector3[] recvData, int id)
    {
        //Debug.Log("Thông số xoay là: " + id + "    " + recvData[0] + "  " + recvData[1]);
        clientIdObjectControl[id].rotate_Control.SetTarget(recvData[0], recvData[1]);
    }
    public void SetRevcMove(string recvData, int id)
    {
        clientIdObjectControl[id].move_Control.ResetValue();

        if (recvData.Length > 0)
        {
            string[] cmd = recvData.Split(' ');
            for (int i = 0; i < cmd.Length; i++)
            {
                if (cmd[i].Length == 0) continue;

                switch (cmd[i][0])
                {
                    case 'X':
                        return;
                    case 'L':
                        clientIdObjectControl[id].move_Control.Left = true;
                        break;
                    case 'R':
                        clientIdObjectControl[id].move_Control.Right = true;
                        break;
                    case 'F':
                        clientIdObjectControl[id].move_Control.Forward = true;
                        break;
                    case 'B':
                        clientIdObjectControl[id].move_Control.Backward = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }

}
