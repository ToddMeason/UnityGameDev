using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public Player player;

    public float healthAmount = 50;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(player.healthPercent < 1)
        {
            player.Heal(healthAmount);
            Destroy(gameObject);
        }
    }
}
