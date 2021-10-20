using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    #region Variables
    public delegate void OnExpChange(float exp, int level);
    public static event OnExpChange OnExpChanged;

    public delegate void OnCurrencyChange(float currency);
    public static event OnCurrencyChange OnCurrencyChanged;

    public int level;
    public float currentLevelExp;
    private float expToLevelUp;

    public float currentCurrency;

    public Gun gun;
    public Rover.Basic.Rover_Controller roverController;
    public Interactable interactable;

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

    int ramDamage = 50;
    int ramDamageTotal = 0;

    int maxVelocityBonusTotal = 0;
    int accelerationBonusTotal = 0;

    #endregion

    #region Builtin Methods
    protected override void Start()
    {
        base.Start();
        LevelUp();
    }

    private void Update()
    {

    }
    #endregion

    #region Custom Methods
    public void SetStats()//not clean but should work, very scuffed
    {
        gun = GetComponent<Rover.Basic.Rover_GunController>().equippedGun;
        roverController = GetComponent<Rover.Basic.Rover_Controller>();
        ramDamageTotal = ramDamage + dmgBonusTotal;

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

                case Stats.maxVelocityBonus:
                    stats[i].baseValue = roverController.maxVelocity;
                    stats[i].bonusValue = maxVelocityBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    roverController.maxVelocityBonus = stats[i].bonusValue;
                    break;

                case Stats.accelerationBonus:
                    stats[i].baseValue = roverController.forwardAcceleration;
                    stats[i].bonusValue = accelerationBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    roverController.forwardAccelerationBonus = stats[i].bonusValue;

                    stats[i].baseValue = roverController.reverseAcceleration;
                    stats[i].bonusValue = accelerationBonusTotal;
                    stats[i].totalValue = stats[i].bonusValue + stats[i].baseValue;
                    roverController.reverseAccelerationBonus = stats[i].bonusValue;
                    break;

                default:
                    break;
            }           
        }
    }

    public void GetBonusStats()
    {
        dmgBonusTotal = 0;
        muzzleVelocityBonusTotal = 0;
        msBetweenShotsBonusTotal = 0;
        maxMagSizeBonusTotal = 0;
        reloadSpeedBonusTotal = 0;
        projectileCountBonusTotal = 0;
        spreadBonusTotal = 0;

        maxVelocityBonusTotal = 0;
        accelerationBonusTotal = 0;


        for (int i = 0; i < inventory.Container.Slots.Count; i++)
        {
            for (int j = 0; j < inventory.Container.Slots[i].item.buffs.Length; j++)
            {
                int value = inventory.Container.Slots[i].item.buffs[j].value;
                int amount = inventory.Container.Slots[i].amount;

                switch (inventory.Container.Slots[i].item.buffs[j].stats)
                {
                    case Stats.dmgBonus:
                        dmgBonusTotal += value * amount;
                        break;
                    case Stats.muzzleVelocityBonus:
                        muzzleVelocityBonusTotal += value * amount;
                        break;
                    case Stats.msBetweenShotsBonus:
                        msBetweenShotsBonusTotal += value * amount;
                        break;
                    case Stats.maxMagSizeBonus:
                        maxMagSizeBonusTotal += value * amount;
                        break;
                    case Stats.reloadSpeedBonus:
                        reloadSpeedBonusTotal += value * amount;
                        break;
                    case Stats.projectileCountBonus:
                        projectileCountBonusTotal += value * amount;
                        break;
                    case Stats.spreadBonus:
                        spreadBonusTotal += value * amount;
                        break;
                    case Stats.maxVelocityBonus:
                        maxVelocityBonusTotal += value * amount;
                        break;
                    case Stats.accelerationBonus:
                        accelerationBonusTotal += value * amount;
                        break;
                    default:
                        break;
                }
            }
        }

        //Debug.Log(dmgBonusTotal);
        SetStats();
    }

    public void TryInteract()
    {
        if (interactable)
        {
            if (interactable.tag == "HasCost")
            {
                if(TrySpendCostAmount(interactable.cost))
                {
                    interactable.Interact();
                }
                else
                {
                    Debug.Log("Not enough money");
                }
            }
            else if (interactable.tag == "NoCost")
            {
                interactable.Interact();
            }         
        }
        else
        {
            Debug.Log("No interactable in range");
        }
    }

    public void AddCurrency(float currency)
    {
        currentCurrency += currency;

        OnCurrencyChanged?.Invoke(currentCurrency);
    }

    public bool TrySpendCostAmount(float cost)
    {
        if (currentCurrency >= cost)
        {
            currentCurrency -= cost;
            OnCurrencyChanged?.Invoke(currentCurrency);
            //trigger chest event to open
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddExp(float exp)
    {
        currentLevelExp += exp;
        if (currentLevelExp >= expToLevelUp)
        {
            currentLevelExp -= expToLevelUp;
            LevelUp();
        }

        OnExpChanged?.Invoke(currentLevelExp / expToLevelUp, level);
    }

    private void LevelUp()
    {
        level++;
        expToLevelUp = level * 50 + Mathf.Pow(level * 2, 2);

        AddExp(0);
    }

    private void OnTriggerEnter(Collider collider)//Detect items and interactables
    {
        if (collider.GetComponent<GroundItem>())
        {
            var item = collider.GetComponent<GroundItem>();
            inventory.AddItem(new Item(item.item), 1);
            Destroy(collider.gameObject);
        }

        if (collider.GetComponent<Interactable>())
        {
            interactable = collider.GetComponent<Interactable>();
        }

        if (collider.GetComponent<Enemy>() && roverController.boosting == true)//works but should add physics to hits later ie activate ragdolls
        {
            collider.GetComponent<Enemy>().TakeDamage(ramDamageTotal);
            Debug.Log("Rammed " + collider.GetComponent<Enemy>().name);
        }
    }

    private void OnTriggerExit(Collider colliderExit)
    {
        if (colliderExit.GetComponent<Interactable>())//Detects if you left the intereactable area then sets it back to null
        {
            interactable = null;
        }      
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnApplicationQuit()//Clears inventory when app is closed, have to check later if this breaks save file
    {
        inventory.Clear();
    }

    public void OnLoad()
    {
        OnExpChanged?.Invoke(currentLevelExp / expToLevelUp, level);
        GetBonusStats();
        OnCurrencyChanged?.Invoke(currentCurrency);
        roverController.SetTotals();
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

    public void SetParent(Player _parent)
    {
        parent = _parent;
        totalValue = baseValue + bonusValue;
    }
}
