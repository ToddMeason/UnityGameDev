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
        public float maxVelocity = 10;                public float maxVelocityBonus = 0;             public float maxVelocityTotal;

        public float forwardAcceleration = 4000;      public float forwardAccelerationBonus = 0;     public float forwardAccelerationTotal;
        public float reverseAcceleration = 2000;      public float reverseAccelerationBonus = 0;     public float reverseAccelerationTotal;
        [SerializeField] private float thrust = 0f;

        public float turnStrength = 250;              public float turnStrengthBonus = 0;            public float turnStrengthTotal;
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

        public bool useController;
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
            if (useController)
            {
                reticleTransform.gameObject.SetActive(false);
            }

            SetTotals();
        }

        private void Update()
        {
            if(Input.GetButton("Shoot") || Input.GetAxis("Shoot") > 0.2f)
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
                thrust = acceleration * forwardAccelerationTotal;
            else if (acceleration < -deadZone)
                thrust = acceleration * reverseAccelerationTotal;

            // Get turning input
            turnValue = 0.0f;
            float turnAxis = Input.GetAxis("Horizontal");
            if (Mathf.Abs(turnAxis) > deadZone)
                turnValue = turnAxis;

            //Debug.Log(turnAxis);
            //Debug.Log(acceleration);


            //Controller input

        }

        void FixedUpdate()
        {
            if(body && input && !useController)//Keyboard Inputs
            {
                HandleMovement();
                HandleTurretKeyboard();
                HandleReticle();
            }
            else if (body && input && useController)
            {
                HandleMovement();
                HandleTurretController();
            }
            else
            {
                Debug.Log("No control inputs detected. Check useContoller bool");
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
                body.AddRelativeTorque(Vector3.up * turnValue * turnStrengthTotal);
            }
            else if (turnValue < 0)
            {
                body.AddRelativeTorque(Vector3.up * turnValue * turnStrengthTotal);
            }


            // Limit max velocity
            if (body.velocity.sqrMagnitude > (body.velocity.normalized * maxVelocityTotal).sqrMagnitude)
            {
                body.velocity = body.velocity.normalized * maxVelocityTotal;
            }
        }

        protected virtual void HandleReticle()
        {
            if(reticleTransform)
            {
                reticleTransform.position = input.ReticlePosition;
            }
        }

        protected virtual void HandleTurretKeyboard()
        {
            if(turretTransform)
            {
                Vector3 turretLookDir = input.ReticlePosition - turretTransform.position;
                turretLookDir.y = 0;

                finalTurretLookDir = Vector3.Slerp(finalTurretLookDir, turretLookDir, Time.deltaTime * turretLagSpeed);
                turretTransform.rotation = Quaternion.LookRotation(finalTurretLookDir);
            }
        }

        protected virtual void HandleTurretController()
        {
            if (turretTransform)
            {
                Vector3 turretLookDir = Vector3.right * Input.GetAxisRaw("RHorizontal") + Vector3.forward * -Input.GetAxisRaw("RVertical");
                if (turretLookDir.sqrMagnitude > 0.0f)
                {
                    turretTransform.rotation = Quaternion.LookRotation(turretLookDir, Vector3.up);
                }
            }
        }

        public void Boost()
        {
            //Setup a boost system that moves the player forward faster based on movespeed and make the player invincible and able to damage enemies 
        }

        public void SetTotals()
        {
            maxVelocityTotal = maxVelocity + maxVelocityBonus;
            forwardAccelerationTotal = forwardAcceleration + forwardAccelerationBonus;
            reverseAccelerationTotal = reverseAcceleration + reverseAccelerationBonus;
            turnStrengthTotal = turnStrength + turnStrengthBonus;
        }

        #endregion

        #region Events
        private void OnEnable()
        {
            InventoryObject.pickedUpItem += SetTotals;
        }

        private void OnDisable()
        {
            InventoryObject.pickedUpItem -= SetTotals;
        }
        #endregion
    }
}
