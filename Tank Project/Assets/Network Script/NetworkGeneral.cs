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

    // [Các Client] [Các Client được active được gửi đến phiên của client đang xét]
    private bool[][] fireData = new bool[256][];
    private bool[][] changeFireData = new bool[256][];
    private Queue<byte[]>[][] damagaData = new Queue<byte[]>[256][];
    // [Các Client] [Các client đó có lệnh nào được gọi]
    private bool[][] activeList = new bool[256][];
    enum Active
    {
        Move,
        Fire,
        ChangeFire,
        Damage
    }


    public void Initialize()
    {
        for (int id = 1; id < clientIdObjectControl.Count; id++)
        {
            clientIdObjectControl[id].SetID(id);
            sendMoveData[id] = new();
            sendRotData[id] = new();
            activeList[id] = new bool[Enum.GetValues(typeof(Active)).Length];
            fireData[id] = new bool[clientIdObjectControl.Count + 1];
            changeFireData[id] = new bool[clientIdObjectControl.Count + 1];
            damagaData[id] = new Queue<byte[]>[clientIdObjectControl.Count + 1];
            for (int j = 1; j< clientIdObjectControl.Count; j++)
            {
                damagaData[id][j] = new Queue<byte[]>();
            }
        }
    }
    public void Awake()
    {
        Initialize();
    }

    //Reveice
    public int RecvData(byte[] encodedData)
    {
        int current_client = -1;
        foreach (var (command, id, dataLength, data) in DecodeWithCheckByte(encodedData))
        {
            //Debug.Log("Lệnh gửi đến server là: " + command + " id la: " + id);
            if (current_client == -1) current_client = id;
            switch (command)
            {
                case (byte)Command.Move:
                    SetRevcMove(DecodeMoveData(data), id);
                    SetActiveListValue(Active.Move, id);
                    //activeList[(int)Active.Move] = true;
                    break;
                case (byte)Command.Rotate:
                    SetRevcRotate(DecodeRotateData(data), id);
                    SetActiveListValue(Active.Move, id);
                    //activeList[(int)Active.Move] = true;
                    break;
                case (byte)Command.Fire:
                    if (SetRevcFire(id))
                    {
                        ///SetActiveListValue
                        SetActiveListValue(Active.Fire, id);
                        Debug.Log("Đã bắn");
                    }

                    break;
                case (byte)Command.ChangeFire:
                    if (SetRevcChangeFire(id))
                    {
                        ///SetActiveListValue
                        SetActiveListValue(Active.ChangeFire, id);
                        Debug.Log("Đã nhận dữ liệu đổi súng");
                    }

                    break;
            }
        }
        Debug.Log("dữ liệu id là: " + current_client);
        return current_client;
    }

    //Get Data ResPond
    public byte[] GetDataRespond(int current_client)
    {
        List<byte> respond = new();
        for (int act = 0; act < Enum.GetValues(typeof(Active)).Length; act++)
        {
            if (activeList[current_client][act])
            {
                switch ((Active)act)
                {
                    case Active.Move:
                        respond.AddRange(GetMoveDataRespond());
                        break;
                    case Active.Fire:
                        respond.AddRange(GetFireInteractDataRespond(current_client));
                        break;
                    case Active.ChangeFire:
                        respond.AddRange(GetChangeFireInteractDataRespond(current_client));
                        break;
                    case Active.Damage:
                        respond.AddRange(GetDamageDataRespond(current_client));
                        break;
                }

                activeList[current_client][act] = false;
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
    /// <summary>
    /// Lấy dữ liệu bắn cho client idx
    /// </summary>
    /// <param name="idx">mã client</param>
    /// <returns></returns>
    public List<byte> GetFireInteractDataRespond(int current_client)
    {
        List<byte> respond = new();

        for (byte id = 1; id < clientIdObjectControl.Count; id++)
        {
            if (fireData[current_client][id])
            {
                fireData[current_client][id] = false;
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

    /// <summary>
    /// Lấy dữ liệu đổi súng cho client idx
    /// </summary>
    /// <param name="idx">mã client</param>
    /// <returns></returns>
    public List<byte> GetChangeFireInteractDataRespond(int current_client)
    {
        List<byte> respond = new();

        for (byte id = 1; id < clientIdObjectControl.Count; id++)
        {
            if (changeFireData[current_client][id])
            {
                changeFireData[current_client][id] = false;
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

    public List<byte> GetDamageDataRespond(int current_client)
    {
        List<byte> respond = new();

        //Debug.Log("Đã truy cập vào Damage");
        for (byte id = 1; id < clientIdObjectControl.Count; id++)
        {
            while (damagaData[current_client][id].Count > 0)
            {
                //Debug.Log("Đã gửi đi giá trị bị dính damage của " + id);
                var value = damagaData[current_client][id].Dequeue();
                ///[command]:   1 byte
                respond.Add((byte)Command.Damage);      
                ///[id]:        1 byte
                respond.Add(id);
                ///[length]:    2 byte
                respond.AddRange(EncodeIntTo2Bytes(value.Length));
                ///[data]:     12 byte
                respond.AddRange(value);
            }

        }

        //Debug.Log("Kết thúc gửi đi giá trị........... ");

        return respond;
    }

    /// <summary>
    /// Set giá trị hành động tương tác
    /// </summary>
    /// <param name="act">mã hành động</param>
    /// <param name="id"> mã id có hành động tương tác đó </param>
    private void SetActiveListValue(Active act, int id)
    {
        switch (act)
        {
            case Active.Move:
                // Active
                for (int client = 1; client < clientIdObjectControl.Count; client++)
                {
                    activeList[client][(int)act] = true;
                }
                break;
            case Active.Fire:
                //  i: các lưu trữ dữ liệu client
                // id: mã id được cần lưu vào 
                for (int client = 1; client < clientIdObjectControl.Count; client++)
                {
                    activeList[client][(int)act] = true;
                    fireData[client][id] = true;
                }
                break;
            case Active.ChangeFire:
                //  i: các lưu trữ dữ liệu client
                // id: mã id được cần lưu vào
                for (int client = 1; client < clientIdObjectControl.Count; client++)
                {
                    activeList[client][(int)act] = true;
                    changeFireData[client][id] = true;
                }
                break;
        }
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
    public void SetDamageDataRespond(byte[] data, int id)
    {
        Debug.Log("Đã cập nhật damage");
        for (int client = 1; client < clientIdObjectControl.Count; client++)
        {
            activeList[client][(int)Active.Damage] = true;
            damagaData[client][id].Enqueue(data);
        }
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
    public bool SetRevcFire(int id) => clientIdObjectControl[id].interact_Control.Fire();
    public bool SetRevcChangeFire(int id) => clientIdObjectControl[id].interact_Control.ChangeFire();
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
