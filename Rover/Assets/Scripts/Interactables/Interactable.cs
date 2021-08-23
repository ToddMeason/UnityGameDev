using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(SphereCollider))]
public abstract class Interactable : MonoBehaviour
{
    public Outline outline;
    public bool activated = false;

    protected virtual void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void Reset()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    //make sure it has ontrigger spherecollider

    public abstract void Interact();

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Player>() && !activated)//Checks if player is within range and if the interactable is still active
        {
            outline.enabled = true;
            //Display interact key(E) above object
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Player>())
        {
            outline.enabled = false;
        }
    }

}
