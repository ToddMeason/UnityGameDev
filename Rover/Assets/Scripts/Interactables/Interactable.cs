using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(SphereCollider))]
public abstract class Interactable : MonoBehaviour
{
    public Outline outline;
    public TextMeshPro textPopup;
    public bool activated = false;

    protected virtual void Awake()
    {
        outline = GetComponent<Outline>();
        textPopup = GetComponentInChildren<TextMeshPro>();
        outline.enabled = false;
        textPopup.enabled = false;
    }

    private void Reset()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    //make sure it has ontrigger spherecollider

    public virtual void Interact()
    {
        if (activated)
        {
            outline.enabled = false;
            textPopup.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Player>() && !activated)//Checks if player is within range and if the interactable is still active
        {
            outline.enabled = true;
            //Display interact key(E) above object
            textPopup.enabled = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Player>())
        {
            outline.enabled = false;
            textPopup.enabled = false;
        }
    }

}
