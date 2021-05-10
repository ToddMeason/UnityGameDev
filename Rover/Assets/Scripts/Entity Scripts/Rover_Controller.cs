using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rover.Basic
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Rover_Inputs))]
    public class Rover_Controller : MonoBehaviour
    {
        #region Variables
        [Header("Movement Properties")]
        public float roverSpeed = 5f;
        public float roverRotationSpeed = 20f;

        [Header("Reticle Properties")]
        public Transform reticleTransform;

        [Header("Turret Properties")]
        public Transform turretTransform;
        public float turretLagSpeed = 0.5f;

        private Rigidbody rb;
        private Rover_Inputs input;
        private Vector3 finalTurretLookDir;
        private Rover_GunController gunController;
        #endregion


        #region Builtin Methods
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<Rover_Inputs>();
            gunController = GetComponent<Rover_GunController>();
        }

        private void Update()
        {
            if(Input.GetButton("Shoot"))
            {
                gunController.Shoot();
            }
        }

        void FixedUpdate()
        {
            if(rb && input)
            {
                HandleMovement();
                HandleTurret();
                HandleReticle();
            }
        }
        #endregion


        #region Custom Methods
        protected virtual void HandleMovement()
        {
            //Move Rover forward
            Vector3 wantedPosition = transform.position + (transform.forward * input.ForwardInput * roverSpeed * Time.deltaTime);
            rb.MovePosition(wantedPosition);

            //Rotate Rover
            Quaternion wantedRotaion = transform.rotation * Quaternion.Euler(Vector3.up * roverRotationSpeed * input.RotationInput * Time.deltaTime);
            rb.MoveRotation(wantedRotaion);
        }

        protected virtual void HandleReticle()
        {
            if(reticleTransform)
            {
                reticleTransform.position = input.ReticlePosition;
            }
        }

        protected virtual void HandleTurret()
        {
            if(turretTransform)
            {
                Vector3 turretLookDir = input.ReticlePosition - turretTransform.position;
                turretLookDir.y = 0;

                finalTurretLookDir = Vector3.Slerp(finalTurretLookDir, turretLookDir, Time.deltaTime * turretLagSpeed);
                turretTransform.rotation = Quaternion.LookRotation(finalTurretLookDir);
            }
        }

        #endregion
    }
}
