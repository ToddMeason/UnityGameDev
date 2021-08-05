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
    #endregion

    #region Builtin Methods
    protected override void Start()
    {
        base.Start();
        gui = FindObjectOfType<Game_GUI>();
        //gui = GameObject.Find("ExpBarBackground").GetComponent<Game_GUI>();//Finds direct gameobject not ideal but works
        LevelUp();
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

    private void OnApplicationQuit()//Clears inventory when app is closed, have to check later if this breaks save file
    {
        inventory.Container.Items.Clear();
    }

    #endregion
}
