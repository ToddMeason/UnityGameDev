using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public GameObject[] items;

    public override void Interact()
    {
        if (!activated)
        {
            Debug.Log("Interacted with chest");

            //Randomly pick and item from the items list then spawn it. Will add rarity later

            GameObject newItem = Instantiate(items[Random.Range(0, items.Length)], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);//Pops item out above chest a little and throws it forward
            newItem.GetComponent<Rigidbody>().AddForce(transform.forward * 200 + transform.up *200);

            activated = true;            
            outline.enabled = false;
        }
    }
}
