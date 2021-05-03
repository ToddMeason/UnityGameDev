using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset;
    public float followSpeed = 10;
    public float lookSpeed = 10;

    public void LookAtTarget()
    {
        Vector3 lookDirection = objectToFollow.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.position, rot, lookSpeed * Time.deltaTime);
    }

    public void MoveToTarget()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookAtTarget();
        MoveToTarget();
    }
}
