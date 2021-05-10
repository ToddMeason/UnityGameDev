using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rover.Basic
{
    [RequireComponent (typeof(AudioSource))]
    public class Rover_GunController : MonoBehaviour
    {
        #region Variables
        public Transform gunLocation;// The game object where the weapon will sit and pivot
        public Gun startingGun;
        private Gun equippedGun;

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
        public void EquipGun(Gun gunToEquip)
        {
            if (equippedGun != null)
            {
                Destroy(equippedGun.gameObject);
            }
            equippedGun = Instantiate(gunToEquip, gunLocation.position, gunLocation.rotation) as Gun;
            equippedGun.transform.parent = gunLocation;
        }

        public void Shoot()
        {
            if (equippedGun != null)
            {
                equippedGun.Shoot();
            }
        }

        #endregion
    }
}
