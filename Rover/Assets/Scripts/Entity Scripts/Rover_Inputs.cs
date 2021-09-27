using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rover.Basic
{
    public class Rover_Inputs : MonoBehaviour
    {
        #region Variables
        public new Camera camera;
        public LayerMask groundLayer;
        #endregion

        #region Properties
        private Vector3 reticlePosition;
        public Vector3 ReticlePosition
        {
            get { return reticlePosition; }
        }

        private Vector3 reticleNormal;
        public Vector3 ReticleNormal
        {
            get { return reticleNormal; }
        }
        #endregion


        #region Builtin Methods
        void Update()
        {
            if(camera)
            {
                HandleInputs();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(reticlePosition, 0.5f);
        }
        #endregion

        #region Custom Methods
        protected virtual void HandleInputs()
        {
            Ray screenRay = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(screenRay, out hit, Mathf.Infinity, groundLayer))
            {
                reticlePosition = hit.point;
                reticleNormal = hit.normal;
            }
        }
        #endregion
    }
}
