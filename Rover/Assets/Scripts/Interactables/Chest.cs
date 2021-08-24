using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public GameObject[] items;
    public Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        float rand = Random.Range(0, 100);

        if (rand < 60)//common
        {
            cost = 25;
            renderer.material.color = Color.gray;
        }
        else if (rand > 60 && rand < 90)//uncommon
        {
            cost = 50;
            renderer.material.color = Color.blue;
        }
        else if (rand > 90)//Rare
        {
            cost = 100;
            renderer.material.color = Color.black;
        }

        textPopup.autoSizeTextContainer = true;
        textPopup.text = "E\n" + cost;
    }

    public override void Interact()
    {
        if (!activated)//add event check here as well
        {
            //Randomly pick and item from the items list then spawn it. Will add rarity later
            GameObject newItem = Instantiate(items[Random.Range(0, items.Length)], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);//Pops item out above chest a little and throws it forward
            newItem.GetComponent<Rigidbody>().AddForce(transform.forward * 200 + transform.up *200);

            activated = true;                       
        }

        base.Interact();
    }
}
