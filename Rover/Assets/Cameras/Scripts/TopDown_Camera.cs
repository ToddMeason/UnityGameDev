using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rover.Cameras
{
    public class TopDown_Camera : MonoBehaviour
    {
        #region Variables
        public Transform target;

        public float height = 20f;
        public float distance = 15f;
        public float angle = 45f;
        public float smoothSpeed = 0.5f;

        private Vector3 refVelocity;
        #endregion


        #region Builtin Methods
        void Start()
        {
            HandleCamera();
        }

        void FixedUpdate()
        {
            HandleCamera();
        }
        #endregion

        #region Custom Methods
        protected virtual void HandleCamera()
        {
            if(!target)
            {
                return;
            }

            //Build worldPosition vector
            Vector3 worldPosistion = (Vector3.forward * -distance) + (Vector3.up * height);
            //Debug.DrawLine(target.position, worldPosistion, Color.red);

            //Build rotatedVector
            Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosistion;
            //Debug.DrawLine(target.position, rotatedVector, Color.green);

            //Move camera position
            Vector3 flatTargetPosition = target.position;
            flatTargetPosition.y = 0;
            Vector3 finalPosition = flatTargetPosition + rotatedVector;
            //Debug.DrawLine(target.position, finalPosition, Color.blue);

            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, smoothSpeed);
            transform.LookAt(target.position);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, 0.25f);
            if (target)
            {
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.DrawSphere(target.position, .5f);
            }
            Gizmos.DrawSphere(transform.position, .5f);
        }
        #endregion
    }
}
