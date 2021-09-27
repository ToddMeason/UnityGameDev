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
        private Rigidbody body;
        private float deadZone = 0f;
        public float groundedDrag = 3f;
        public float maxVelocity = 50;

        public float forwardAcceleration = 8000f;
        public float reverseAcceleration = 4000f;
        [SerializeField] private float thrust = 0f;

        public float turnStrength = 1000f;
        float turnValue = 0f;

        public ParticleSystem[] dustTrails = new ParticleSystem[2];

        [Header("Reticle Properties")]
        public Transform reticleTransform;

        [Header("Turret Properties")]
        public Transform turretTransform;
        public float turretLagSpeed = 0.5f;

        private Rover_Inputs input;
        private Vector3 finalTurretLookDir;
        private Rover_GunController gunController;
        protected Player player;
        #endregion


        #region Builtin Methods
        void Start()
        {
            body = GetComponent<Rigidbody>();
            input = GetComponent<Rover_Inputs>();
            gunController = GetComponent<Rover_GunController>();
            player = GetComponent<Player>();
            body.centerOfMass = Vector3.down;
            //boostSpeed = roverSpeed * 5;
        }

        private void Update()
        {
            if(Input.GetButton("Shoot"))
                gunController.Shoot();

            if(Input.GetButton("Reload"))
                gunController.Reload();

            if (Input.GetButtonDown("Interact"))
                player.TryInteract();

            if(Input.GetButton("Sprint"))
                Boost();


            // Get thrust input
            thrust = 0.0f;
            float acceleration = Input.GetAxis("Vertical");
            if (acceleration > deadZone)
                thrust = acceleration * forwardAcceleration;
            else if (acceleration < -deadZone)
                thrust = acceleration * reverseAcceleration;

            // Get turning input
            turnValue = 0.0f;
            float turnAxis = Input.GetAxis("Horizontal");
            if (Mathf.Abs(turnAxis) > deadZone)
                turnValue = turnAxis;

            //Debug.Log(turnAxis);
            //Debug.Log(acceleration);

        }

        void FixedUpdate()
        {
            if(body && input)
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
            //add something to make emmission stop if not moving
            var emissionRate = 10;
            body.drag = groundedDrag;

            for (int i = 0; i < dustTrails.Length; i++)
            {
                var emission = dustTrails[i].emission;
                emission.rateOverTime = new ParticleSystem.MinMaxCurve(emissionRate);
            }

            // Handle Forward and Reverse forces
            if (Mathf.Abs(thrust) > 0)
                body.AddForce(transform.forward * thrust);

            // Handle Turn forces
            if (turnValue > 0)
            {
                body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
            }
            else if (turnValue < 0)
            {
                body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
            }


            // Limit max velocity
            if (body.velocity.sqrMagnitude > (body.velocity.normalized * maxVelocity).sqrMagnitude)
            {
                body.velocity = body.velocity.normalized * maxVelocity;
            }
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

        public void Boost()
        {
            //Setup a boost system that moves the player forward faster based on movespeed and make the player invincible and able to damage enemies 
        }

        #endregion
    }
}
