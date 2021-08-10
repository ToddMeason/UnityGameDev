using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    #region Variables
    private int level;
    private float currentLevelExp;
    private float expToLevelUp;

    private Game_GUI gui;
    public Gun gun;

    public InventoryObject inventory;
    public Stat[] stats;

    #endregion

    #region Builtin Methods
    protected override void Start()
    {
        base.Start();
        gui = FindObjectOfType<Game_GUI>();       
        LevelUp();
        //gun = GetComponent<Rover.Basic.Rover_GunController>().equippedGun;
        //if (gun)
        //{
        //    SetBaseStats();
        //}     
    }

    private void Update()
    {
        UpdateHealth();
        //gun = GetComponent<Rover.Basic.Rover_GunController>().equippedGun;//need to add to equip gun event later not update every frame.
    }
    #endregion

    #region Custom Methods
    public void SetBaseStats()//not clean but should work
    {
        gun = GetComponent<Rover.Basic.Rover_GunController>().equippedGun;

        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].SetParent(this);
            switch (stats[i].type)
            {
                case Stats.dmgBonus: stats[i].baseValue = gun.dmg;
                    break;

                case Stats.muzzleVelocityBonus:
                    stats[i].baseValue = gun.muzzleVelocity;
                    break;

                case Stats.msBetweenShotsBonus:
                    stats[i].baseValue = gun.msBetweenShots;
                    break;

                case Stats.maxMagSizeBonus:
                    stats[i].baseValue = gun.maxMagSize;
                    break;

                case Stats.reloadSpeedBonus:
                    stats[i].baseValue = gun.reloadSpeed;
                    break;

                case Stats.projectileCountBonus:
                    stats[i].baseValue = gun.projectileCount;
                    break;

                case Stats.spreadBonus:
                    stats[i].baseValue = gun.spread;
                    break;
            }           
        }
    }

    public void UpdateHealth()
    {
        gui.ShowHealth(healthPercent);
    }

    public void AddExp(float exp)
    {
        currentLevelExp += exp;
        if (currentLevelExp >= expToLevelUp)
        {
            currentLevelExp -= expToLevelUp;
            LevelUp();
        }

        gui.SetPlayerExp(currentLevelExp / expToLevelUp, level);
    }

    private void LevelUp()
    {
        level++;
        expToLevelUp = level * 50 + Mathf.Pow(level * 2, 2);

        AddExp(0);
    }

    private void OnTriggerEnter(Collider other)//Pick up items and add to inventory
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1);
            Destroy(other.gameObject);
        }
    }

    public void StatModified(Stat stat)
    {
        Debug.Log(stat.type + " was updated! Value is now " + stat.totalValue);
    }

    private void OnApplicationQuit()//Clears inventory when app is closed, have to check later if this breaks save file
    {
        inventory.Clear();
    }

    #endregion

    #region Events
    private void OnEnable()
    {
        Rover.Basic.Rover_GunController.GunEquipped += SetBaseStats;
    }

    private void OnDisable()
    {
        Rover.Basic.Rover_GunController.GunEquipped -= SetBaseStats;
    }
    #endregion
}

[System.Serializable]
public class Stat
{
    [SerializeField]
    public Player parent;
    public Stats type;
    public float baseValue;
    public float bonusValue;
    public float totalValue;
    //public ModifiableInt value;

    public void SetParent(Player _parent)
    {
        parent = _parent;
        totalValue = baseValue + bonusValue;
        //value = new ModifiableInt(StatModified);
    }

    public void StatModified()
    {
        parent.StatModified(this);
    }
}



//SetBaseStats
//for (int i = 0; i < stats.Length; i++)
//{
//    stats[i].SetParent(this);

//    if (stats[i].type == Stats.dmgBonus)
//    {
//        stats[i].baseValue = gun.dmg;
//    }
//    else if (stats[i].type == Stats.muzzleVelocityBonus)
//    {
//        stats[i].baseValue = gun.muzzleVelocity;
//    }
//    else if (stats[i].type == Stats.msBetweenShotsBonus)
//    {
//        stats[i].baseValue = gun.msBetweenShots;
//    }
//    else if (stats[i].type == Stats.maxMagSizeBonus)
//    {
//        stats[i].baseValue = gun.maxMagSize;
//    }
//    else if (stats[i].type == Stats.reloadSpeedBonus)
//    {
//        stats[i].baseValue = gun.reloadSpeed;
//    }
//    else if (stats[i].type == Stats.projectileCountBonus)
//    {
//        stats[i].baseValue = gun.projectileCount;
//    }
//    else if (stats[i].type == Stats.spreadBonus)
//    {
//        stats[i].baseValue = gun.spread;
//    }
//    else
//    {
//        Debug.Log("Not a stat type on player");
//    }
//}