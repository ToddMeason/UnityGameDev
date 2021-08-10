using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rover.Basic
{
    [RequireComponent (typeof(AudioSource))]
    public class Rover_GunController : MonoBehaviour
    {
        #region Variables
        public delegate void EquippedGun();
        public static event EquippedGun GunEquipped;

        public Transform gunLocation;// The game object where the weapon will sit and pivot
        public Gun startingGun;
        public Gun equippedGun;

        #endregion

        #region Builtin Methods
        private void Start()
        {
            if (startingGun != null)
            {
                EquipGun(startingGun);
            }
        }
        #endregion

        #region Custom Methods
        public void EquipGun(Gun gunToEquip)//need to add equip gun event later
        {
            if (equippedGun != null)
            {
                Destroy(equippedGun.gameObject);
            }
            equippedGun = Instantiate(gunToEquip, gunLocation.position, gunLocation.rotation) as Gun;
            equippedGun.transform.parent = gunLocation;

            GunEquipped?.Invoke();
        }

        public void Shoot()
        {
            if (equippedGun != null)
            {
                equippedGun.Shoot();
            }
        }

        public void Reload()
        {
            if (equippedGun != null)
            {
                equippedGun.Reload();
            }
        }
        #endregion
    }
}
