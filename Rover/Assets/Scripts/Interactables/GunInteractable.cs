using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunInteractable : Interactable
{
    [SerializeField] private Gun gun;
    [SerializeField] private Rover.Basic.Rover_GunController gunController;


    public override void Interact()
    {
        gunController.EquipGun(gun);      
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rover.Basic.Rover_GunController>())
        {
            gunController = other.GetComponent<Rover.Basic.Rover_GunController>();
        }

        base.OnTriggerEnter(other);
    }
}
