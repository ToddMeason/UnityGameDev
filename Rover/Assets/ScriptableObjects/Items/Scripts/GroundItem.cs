using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemObject item;
    public LayerMask terrainLayer;
    public LayerMask groundLayer;

    private void OnTriggerEnter(Collider c)
    {
        //if (c.gameObject.layer == groundLayer || c.gameObject.layer == terrainLayer)//this somehow doesnt work, it has to be like the one below. Thanks Unity very cool
        //{
        //    GetComponent<Rigidbody>().Sleep();
        //}

        if ((groundLayer.value & 1<< c.gameObject.layer) != 0  || (terrainLayer.value & 1 << c.gameObject.layer) != 0)//this works 
        {
            GetComponent<Rigidbody>().Sleep();
        }
    }
}
