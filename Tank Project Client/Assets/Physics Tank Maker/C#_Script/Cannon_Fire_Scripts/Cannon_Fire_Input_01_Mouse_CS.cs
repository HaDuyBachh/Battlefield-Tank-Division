using System.Collections;
using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Cannon_Fire_Input_01_Mouse_CS : Cannon_Fire_Input_00_Base_CS
	{

        protected Turret_Horizontal_CS turretScript;
        private float thresh_hold = 0;
        public bool fire = false;
        public bool changeFire = false;
        public bool isEnable = false;

        public override void Prepare(Cannon_Fire_CS cannonFireScript)
        {
            this.cannonFireScript = cannonFireScript;

            turretScript = GetComponentInParent<Turret_Horizontal_CS>();
        }


        public override void Get_Input()
		{
            if (isEnable)
            {
                // Fire.
                if (thresh_hold > 0) thresh_hold -= Time.deltaTime;
                if (turretScript.Is_Ready && Input.GetKey(General_Settings_CS.Fire_Key) && thresh_hold <= 0)
                {
                    cannonFireScript.SendActiveFire();
                    thresh_hold = cannonFireScript.Reload_Time;
                }

                // Switch the bullet type.
                if (Input.GetKeyDown(General_Settings_CS.Switch_Bullet_Key))
                {
                    cannonFireScript.SendActiveFireChange();
                }
            }

            if (changeFire)
            {
                changeFire = false;
                // Call the "Bullet_Generator_CS" scripts.
                for (int i = 0; i < cannonFireScript.Bullet_Generator_Scripts.Length; i++)
                {
                    if (cannonFireScript.Bullet_Generator_Scripts[i] == null)
                    {
                        continue;
                    }
                    cannonFireScript.Bullet_Generator_Scripts[i].Switch_Bullet_Type();
                }

                // Reload.
                cannonFireScript.StartCoroutine("Reload");
            }

            if (fire)
            {
                fire = false;
                cannonFireScript.Fire();
            }
        }

        public override void NetworkCallFire()
        {
            fire = true;
            Get_Input();
        }

        public override void NetworkCallChangeFire()
        {
            changeFire = true;
            Get_Input();
        }

        public override void SetEnable(bool state)
        {
            isEnable = state;
        }
    }

}
