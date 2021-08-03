using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponStat Item", menuName = "Inventory System/Items/WeaponStat")]
public class WeaponStatObject : ItemObject
{
    //May need to change type later
    public float dmgBonus;              //gun damage per shot
    public float muzzleVelocityBonus;   //Speed of bullet
    public float msBetweenShotsBonus;   //Rate of fire for gun, smaller is faster
    public float maxMagSizeBonus;       //How many bullets in the guns magazine 
    public float reloadSpeedBonus;      //How long it takes to reload, smaller is faster
    public float projectileCountBonus;  //Amount of projectiles fired per shot
    public float spreadBonus;           //Accuracy or spread of bullets, smaller is more accurate

    private void Awake()
    {
        type = ItemType.WeaponStat;
    }
}
