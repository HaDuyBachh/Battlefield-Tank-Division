using System.Collections;
using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Cannon_Fire_Input_01_Mouse_CS : Cannon_Fire_Input_00_Base_CS
	{

        protected Turret_Horizontal_CS turretScript;
        public bool fire = false;
        public bool changeFire = false;

        public override void Prepare(Cannon_Fire_CS cannonFireScript)
        {
            this.cannonFireScript = cannonFireScript;

            turretScript = GetComponentInParent<Turret_Horizontal_CS>();
        }


        public override void Get_Input()
		{
            // Fire.
            //if (turretScript.Is_Ready && Input.GetKey(General_Settings_CS.Fire_Key))
            if (turretScript.Is_Ready && fire)
            {
                fire = false;
                cannonFireScript.Fire();
            }

            // Switch the bullet type.
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
        }

        public override bool NetWorkFire()
        {
            if (turretScript.Is_Ready && cannonFireScript.Is_Loaded) fire = true;
            return (fire);
        }

        public override bool NetWorkChangeFire()
        {
            changeFire = true;
            return (changeFire);
        }



    }

}
