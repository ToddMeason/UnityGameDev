using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemObject item;
    public LayerMask terrainLayer;
    public LayerMask groundLayer;

    void OnTriggerEnter(Collider c)//Detecting ground collision
    {
        //Debug.Log($"Hit {LayerMask.LayerToName(c.gameObject.layer)}");//This detects it is hitting tht correct layer and reading it 
        if (c.gameObject.layer == groundLayer || c.gameObject.layer == terrainLayer)//Dont know why it doesnt pick up layer like this will change later
        {
            GetComponent<Rigidbody>().Sleep();
        }
    }
}
