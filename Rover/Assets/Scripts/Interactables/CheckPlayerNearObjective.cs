using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerNearObjective : MonoBehaviour
{
    public MainObjective mainObjective;

    private void Start()
    {
        mainObjective = GetComponentInParent<MainObjective>();
        GetComponent<ParticleSystem>().Stop();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponent<Player>())//Checks if player is within range
        {
            mainObjective.playerInRange = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Player>())//Checks if player re-enters range and starts the timer again
        {
            mainObjective.playerInRange = true;
            if (mainObjective.activated)
            {
                mainObjective.StartCoroutine(mainObjective.Timer());
            }         
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
