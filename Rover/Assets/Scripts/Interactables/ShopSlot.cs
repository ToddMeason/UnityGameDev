using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSlot : Interactable
{
    public GameObject[] items;//Add rarity later, maybe as another list 
    public GameObject newItem;
    public Renderer renderer;

    public int shopCount = 4;

    void Start()//Need to set item in here later once rarity is setup
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

        DisplayItem();
        textPopup.autoSizeTextContainer = true;
        textPopup.text = "E\n" + cost;
    }

    public void DisplayItem()
    {
        newItem = Instantiate(items[Random.Range(0, items.Length)], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);//Displays item above the shopSlot
        newItem.GetComponent<Rigidbody>().isKinematic = true;
        newItem.GetComponent<GroundItem>().enabled = false;//Turns off components so it doesnt move and cant be picked up
    }

    public override void Interact()
    {
        if (!activated)
        {
            //Randomly pick an item from the items list then spawn it. Will add rarity later
            newItem.GetComponent<Rigidbody>().isKinematic = false;
            newItem.GetComponent<GroundItem>().enabled = true;
            newItem.GetComponent<Rigidbody>().AddForce(transform.forward * 200 + transform.up * 200);

            activated = true;
        }

        base.Interact();
    }
}
