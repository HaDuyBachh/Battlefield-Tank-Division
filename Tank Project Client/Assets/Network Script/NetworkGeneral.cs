using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGeneral : MonoBehaviour
{

    public Network_Move_Control moveControl;
    private string revcStr = "";
    public void SetRevc(string revcStr)
    {
        //this.revcStr = revcStr;

        if (revcStr.Length > 0)
        {
            string[] cmd = revcStr.Split(' ');
            moveControl.ResetValue();
            for (int i = 1; i < cmd.Length; i++)
            {
                if (cmd[i].Length == 0) continue;

                switch (cmd[i][0])
                {
                    case 'L':
                        moveControl.Left = true;
                        break;
                    case 'R':
                        moveControl.Right = true;
                        break;
                    case 'F':
                        moveControl.Forward = true;
                        break;
                    case 'B':
                        moveControl.Backward = true;
                        break;
                    default:
                        break;
                }
            }
            revcStr = "";
        }
    }
}
