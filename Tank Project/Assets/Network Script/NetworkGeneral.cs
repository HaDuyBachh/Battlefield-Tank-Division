using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGeneral : MonoBehaviour
{

    public List<Network_Move_Control> moveControl;

    private List<byte>[] sendStr = new List<byte>[100];
    private void Awake()
    {
        for (int i = 1; i < moveControl.Count; i++)
        {
            moveControl[i].SetID(i);
            moveControl[i].GetComponent<Network_SendPos>().SetID(i);
        }
    }
    public byte[] SendStr
    {
        get
        {
            // Kiểm tra sự tồn tại của sendStr[1] và sendStr[2]
            if (sendStr[1] == null || sendStr[2] == null)
            {
                return null; // Trả về null nếu một trong các mảng không tồn tại
            }

            // Sử dụng List<byte> để ghép các mảng byte
            List<byte> byteList = new List<byte>();

            // Thêm sendStr[1] vào List
            byteList.AddRange(sendStr[1]);

            // Thêm sendStr[2] vào List
            byteList.AddRange(sendStr[2]);

            // Chuyển List<byte> trở lại thành mảng byte
            return byteList.ToArray();
        }
    }

    public void SetRevcRotate(string revcStr, int id)
    {

    }

    public void SetRevcMove(string revcStr, int id)
    {
        Debug.Log("Thông số di chuyển là: " + id + "    " + revcStr);
        moveControl[id].ResetValue();
        if (revcStr.Length > 0)
        {
            string[] cmd = revcStr.Split(' ');
            for (int i = 1; i < cmd.Length; i++)
            {
                if (cmd[i].Length == 0) continue;

                switch (cmd[i][0])
                {
                    case 'L':
                        moveControl[id].Left = true;
                        break;
                    case 'R':
                        moveControl[id].Right = true;
                        break;
                    case 'F':
                        moveControl[id].Forward = true;
                        break;
                    case 'B':
                        moveControl[id].Backward = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public void SetSendStr(byte[] sendStr, int id)
    {
        //this.sendStr[id] = sendStr;
    }
}
