using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public GameObject[] items;//Add rarity later, maybe as another list 

    private void Start()//Need to set item in here later once rarity is setup
    {
        float rand = Random.Range(0, 100);

        if (rand < 60)//common
        {
            cost = 50;
        }
        else if (rand > 60 && rand < 90)//uncommon
        {
            cost = 50;
        }
        else if (rand > 90)//Rare
        {
            cost = 50;
        }

        textPopup.autoSizeTextContainer = true;
        textPopup.text = "A\n" + cost;
    }

    public override void Interact()
    {
        if (!activated)
        {
            //Randomly pick an item from the items list then spawn it. Will add rarity later
            GameObject newItem = Instantiate(items[Random.Range(0, items.Length)], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);//Pops item out above chest a little and throws it forward
            newItem.GetComponent<Rigidbody>().AddForce(transform.forward * 200 + transform.up *200);

            activated = true;
            Destroy(gameObject, 5);
        }

        base.Interact();
    }
}
