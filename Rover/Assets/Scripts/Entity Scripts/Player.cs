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

    public InventoryObject inventory;
    public Stat[] stats;

    #endregion

    #region Builtin Methods
    protected override void Start()
    {
        base.Start();
        gui = FindObjectOfType<Game_GUI>();
        //gui = GameObject.Find("ExpBarBackground").GetComponent<Game_GUI>();//Finds direct gameobject not ideal but works
        LevelUp();

        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].SetParent(this);
        }
        for (int i = 0; i < inventory.Container.Slots.Count; i++)
        {
            //Need to setup OnBeforeUpdate and OnAfterUpdate in inventory and need to access InventorySlot
            //inventory.Container.Items[i].
        }  
    }

    private void Update()
    {
        UpdateHealth();
    }
    #endregion

    #region Custom Methods
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
        Debug.Log(stat.type + " was updated! Value is now " + stat.value.ModifiedValue);
    }

    private void OnApplicationQuit()//Clears inventory when app is closed, have to check later if this breaks save file
    {
        inventory.Clear();
    }

    #endregion
}

[System.Serializable]
public class Stat
{
    [SerializeField]
    public Player parent;
    public Stats type;
    public ModifiableInt value;

    public void SetParent(Player _parent)
    {
        parent = _parent;
        value = new ModifiableInt(StatModified);
    }

    public void StatModified()
    {
        parent.StatModified(this);
    }
}
