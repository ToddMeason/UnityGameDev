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

    public InventoryObject inventory;//might need to auto get later with new scenes
    public Stat[] stats;

    //not the best way of doing this but it should work for now, will need to make easier to add new stats later
    int dmgBonusTotal = 0;
    int muzzleVelocityBonusTotal = 0;
    int msBetweenShotsBonusTotal = 0;
    int maxMagSizeBonusTotal = 0;
    int reloadSpeedBonusTotal = 0;
    int projectileCountBonusTotal = 0;
    int spreadBonusTotal = 0;

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
    }
    #endregion

    #region Custom Methods
    public void SetStats()//not clean but should work, very scuffed
    {
        gun = GetComponent<Rover.Basic.Rover_GunController>().equippedGun;

        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].SetParent(this);
            switch (stats[i].type)
            {
                case Stats.dmgBonus: 
                    stats[i].baseValue = gun.dmg;
                    stats[i].bonusValue = dmgBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    gun.dmgBonus = stats[i].bonusValue;
                    break;

                case Stats.muzzleVelocityBonus:
                    stats[i].baseValue = gun.muzzleVelocity;
                    stats[i].bonusValue = muzzleVelocityBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    gun.muzzleVelocityBonus = stats[i].bonusValue;
                    break;

                case Stats.msBetweenShotsBonus:
                    stats[i].baseValue = gun.msBetweenShots;
                    stats[i].bonusValue = msBetweenShotsBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    gun.msBetweenShotsBonus = stats[i].bonusValue;
                    break;

                case Stats.maxMagSizeBonus:
                    stats[i].baseValue = gun.maxMagSize;
                    stats[i].bonusValue = maxMagSizeBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    gun.maxMagSizeBonus = stats[i].bonusValue;
                    break;

                case Stats.reloadSpeedBonus:
                    stats[i].baseValue = gun.reloadSpeed;
                    stats[i].bonusValue = reloadSpeedBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    gun.reloadSpeedBonus = stats[i].bonusValue;
                    break;

                case Stats.projectileCountBonus:
                    stats[i].baseValue = gun.projectileCount;
                    stats[i].bonusValue = projectileCountBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    gun.projectileCountBonus = stats[i].bonusValue;
                    break;

                case Stats.spreadBonus:
                    stats[i].baseValue = gun.spread;
                    stats[i].bonusValue = spreadBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    gun.spreadBonus = stats[i].bonusValue;
                    break;

                default:
                    break;
            }           
        }
    }

    public void GetBonusStats()
    {
        for (int i = 0; i < inventory.Container.Slots.Count; i++)
        {
            for (int j = 0; j < inventory.Container.Slots[i].item.buffs.Length; j++)
            {
                switch (inventory.Container.Slots[i].item.buffs[j].stats)
                {
                    case Stats.dmgBonus:
                        dmgBonusTotal += inventory.Container.Slots[i].item.buffs[j].value;
                        break;
                    case Stats.muzzleVelocityBonus:
                        muzzleVelocityBonusTotal += inventory.Container.Slots[i].item.buffs[j].value;
                        break;
                    case Stats.msBetweenShotsBonus:
                        msBetweenShotsBonusTotal += inventory.Container.Slots[i].item.buffs[j].value;
                        break;
                    case Stats.maxMagSizeBonus:
                        maxMagSizeBonusTotal += inventory.Container.Slots[i].item.buffs[j].value;
                        break;
                    case Stats.reloadSpeedBonus:
                        reloadSpeedBonusTotal += inventory.Container.Slots[i].item.buffs[j].value;
                        break;
                    case Stats.projectileCountBonus:
                        projectileCountBonusTotal += inventory.Container.Slots[i].item.buffs[j].value;
                        break;
                    case Stats.spreadBonus:
                        spreadBonusTotal += inventory.Container.Slots[i].item.buffs[j].value;
                        break;
                    default:
                        break;
                }
            }
        }

        Debug.Log(dmgBonusTotal);
        SetStats();
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
        InventoryObject.pickedUpItem += GetBonusStats;
        Rover.Basic.Rover_GunController.GunEquipped += SetStats;
    }

    private void OnDisable()
    {
        InventoryObject.pickedUpItem -= GetBonusStats;
        Rover.Basic.Rover_GunController.GunEquipped -= SetStats;
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