using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemObject item;

    void OnTriggerEnter(Collider c)//Detecting ground collision
    {
        if (c.tag == "Ground")
        {
            GetComponent<Rigidbody>().Sleep();
        }
    }
}
