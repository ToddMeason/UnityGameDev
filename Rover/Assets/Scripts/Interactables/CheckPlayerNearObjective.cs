using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerNearObjective : MonoBehaviour
{
    public MainObjective mainObjective;

    private void Start()
    {
        mainObjective = GetComponentInParent<MainObjective>();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponent<Player>())//Checks if player is within range
        {
            mainObjective.playerInRange = true;
            
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Player>())
        {
            mainObjective.playerInRange = false;
        }
    }
}
