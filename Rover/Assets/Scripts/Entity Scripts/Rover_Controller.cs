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
        public delegate void OnBoost(bool boostAvailable);
        public static event OnBoost OnBoostUsed;

        [Header("Movement Properties")]
        private Rigidbody body;
        private float deadZone = 0.2f;
        public float groundedDrag = 3f;
        public float maxVelocity = 10;                public float maxVelocityBonus = 0;             public float maxVelocityTotal;

        public float forwardAcceleration = 4000;      public float forwardAccelerationBonus = 0;     public float forwardAccelerationTotal;
        public float reverseAcceleration = 2000;      public float reverseAccelerationBonus = 0;     public float reverseAccelerationTotal;
        [SerializeField] private float thrust = 0f;

        public float turnStrengthTotal = 250;
        float turnValue = 0f;

        public float boostSpeed;
        public float boostTime = 3;
        public bool boosting;
        public bool boostAvailable;
        public float boostCooldown = 15;

        [SerializeField] private float playerLagSpeed;

        [Header("Reticle Properties")]
        public Transform reticleTransform;

        [Header("Turret Properties")]
        public Transform turretTransform;
        [SerializeField] private float turretLagSpeed = 0.5f;

        private Rover_Inputs input;
        private Vector3 finalTurretLookDir;
        private Vector3 finalPlayerLookDir;
        private Rover_GunController gunController;
        protected Player player;

        [SerializeField] private float cameraOffset = 45;
        public ParticleSystem[] dustTrails = new ParticleSystem[2];
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
            boostSpeed = forwardAccelerationTotal * 4;
            boosting = false;
            boostAvailable = true;
            OnBoostUsed?.Invoke(boostAvailable);

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

            if(Input.GetButtonDown("Sprint") && !boosting && boostAvailable)
                StartCoroutine(Boost());

            if (useController)
            {
                ControllerMovementInput();
            }
            else
            {
                 KeyBoardMovementInput();
            }          
        }


        void FixedUpdate()
        {
            if(body && input && !useController)//Keyboard Inputs
            {
                HandleMovementKeyboard();
                HandleTurretKeyboard();
                HandleReticle();
            }
            else if (body && input && useController)
            {
                HandleMovementController();
                HandleTurretController();
            }
            else
            {
                Debug.Log("No control inputs detected. Check useContoller bool");
            }
        }
        #endregion


        #region Custom Methods
        private void KeyBoardMovementInput()
        {
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
        }

        private void ControllerMovementInput()
        {
            Vector3 wantedPlayerLookDir = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.forward * Input.GetAxisRaw("Vertical");
            if (wantedPlayerLookDir.sqrMagnitude > 0.0f)
            {
                finalPlayerLookDir = Vector3.Slerp(finalPlayerLookDir, wantedPlayerLookDir, Time.deltaTime * playerLagSpeed);
                transform.rotation = Quaternion.LookRotation(finalPlayerLookDir, Vector3.up) * Quaternion.Euler(0, cameraOffset,0);

                thrust = 0.0f;
                float acceleration;
                
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal") - 0) > deadZone || Mathf.Abs(Input.GetAxisRaw("Vertical") - 0) > deadZone)
                {
                    if (Mathf.Abs(Input.GetAxisRaw("Horizontal") - 0) > Mathf.Abs(Input.GetAxisRaw("Vertical") - 0))
                    {
                        acceleration = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
                    }
                    else if (Mathf.Abs(Input.GetAxisRaw("Vertical") - 0) > Mathf.Abs(Input.GetAxisRaw("Horizontal") - 0))
                    {
                        acceleration = Mathf.Abs(Input.GetAxisRaw("Vertical"));
                    }
                    else
                    {
                        acceleration = 0;
                    }
                }
                else
                {
                    acceleration = 0;
                }

                Debug.Log(Input.GetAxisRaw("Horizontal"));
                Debug.Log(Input.GetAxisRaw("Vertical"));


                thrust = acceleration * forwardAccelerationTotal;
            }
        }

        protected virtual void HandleMovementKeyboard()
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

        private void HandleMovementController()
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
                    turretTransform.rotation = Quaternion.LookRotation(turretLookDir, Vector3.up) * Quaternion.Euler(0, cameraOffset, 0);
                }
            }
        }

        public void SetTotals()
        {
            maxVelocityTotal = maxVelocity + maxVelocityBonus;
            forwardAccelerationTotal = forwardAcceleration + forwardAccelerationBonus;
            reverseAccelerationTotal = reverseAcceleration + reverseAccelerationBonus;
            boostSpeed = forwardAccelerationTotal * 4;
        }

        #endregion

        #region Coroutines
        IEnumerator Boost()
        {
            //Setup a boost system that moves the player forward faster based on movespeed and make the player invincible and able to damage enemies 
            boosting = true;
            player.invulnerable = true;
            boostAvailable = false;
            OnBoostUsed?.Invoke(boostAvailable);

            float originalAcc = forwardAccelerationTotal;
            forwardAccelerationTotal = boostSpeed;

            yield return new WaitForSeconds(boostTime);

            forwardAccelerationTotal = originalAcc;
            boosting = false;
            player.invulnerable = false;

            yield return new WaitForSeconds(boostCooldown);
            boostAvailable = true;
            OnBoostUsed?.Invoke(boostAvailable);
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
