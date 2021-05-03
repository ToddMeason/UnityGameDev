using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rover.Basic
{
    [RequireComponent (typeof(AudioSource))]
    public class Rover_Gun : MonoBehaviour
    {
        #region Variables
        public enum GunType { Semi, Auto };
        public GunType gunType;

        public LayerMask collisionMask;//What the bullet can hit
        public float rpm; //rounds per minute
        public float dmg = 10;//gun damage per shot

        public Transform bulletSpawn;//Where bullet comes out ie barrel
        public Rigidbody shell;
        public Transform shellEjectionSpawn;//Where shell casing is ejected
        private LineRenderer tracer;//Gun shooting effect
        
        private float secondsBetweenShots;
        private float nextPossibleShootTime;

        #endregion

        #region Builtin Methods
        private void Start()
        {
            secondsBetweenShots = 60 / rpm;
            if (GetComponent<LineRenderer>())
            {
                tracer = GetComponent<LineRenderer>();
            }
        }
        #endregion

        #region Custom Methods
        public void Shoot()
        {
            if (CanShoot())
            {
                Ray ray = new Ray(bulletSpawn.position, bulletSpawn.forward);
                RaycastHit hit;

                float shotDistance = 20;

                if (Physics.Raycast(ray, out hit, shotDistance, collisionMask))//Detecting if the bullet hit anything
                {
                    shotDistance = hit.distance;

                    if(hit.collider.GetComponent<Entity>())//If hit enemy deal damage
                    {
                        hit.collider.GetComponent<Entity>().TakeDamage(dmg);
                    }
                }

                nextPossibleShootTime = Time.time + secondsBetweenShots;

                GetComponent<AudioSource>().Play();

                if (tracer)//Rendering shot effect
                {
                    StartCoroutine("RenderTracer", ray.direction * shotDistance);
                }

                Rigidbody newShell = Instantiate(shell, shellEjectionSpawn.position, Quaternion.identity) as Rigidbody;//Spawning a shell casing after a shot 
                newShell.AddForce(shellEjectionSpawn.forward * Random.Range(150, 200) + bulletSpawn.forward * Random.Range(-10, 10));
            }  
        }

        public void ShootContinous()
        {
            if (gunType == GunType.Auto)
            {
                Shoot();
            }
        }

        private bool CanShoot()
        {
            bool canShoot = true;

            if (Time.time < nextPossibleShootTime)
            {
                canShoot = false;
            }

            return canShoot;
        }

        #endregion

        #region Coroutines
        IEnumerator RenderTracer(Vector3 hitPoint)
        {
            tracer.enabled = true;
            tracer.SetPosition(0, bulletSpawn.position);
            tracer.SetPosition(1, bulletSpawn.position + hitPoint);
            yield return null;//Sets to render for one frame, check frame rate if not rendering properly
            tracer.enabled = false;
        }
        #endregion
    }
}
