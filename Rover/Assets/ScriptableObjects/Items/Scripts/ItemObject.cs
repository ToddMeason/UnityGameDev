using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType 
{
    PlayerStat,
    WeaponStat
}

public enum Stats//Add player stats and weapon stats in here
{
    dmgBonus,              //gun damage per shot
    muzzleVelocityBonus,   //Speed of bullet
    msBetweenShotsBonus,   //Rate of fire for gun, smaller is faster
    maxMagSizeBonus,       //How many bullets in the guns magazine 
    reloadSpeedBonus,      //How long it takes to reload, smaller is faster
    projectileCountBonus,  //Amount of projectiles fired per shot
    spreadBonus           //Accuracy or spread of bullets, smaller is more accurate
}

public abstract class ItemObject : ScriptableObject//Base item script for stat changes above for more complex items later will need to create special item types with there own methods
{
    public int id;
    public Sprite uiIcon;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public ItemBuff[] buffs;

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id;
    public ItemBuff[] buffs;
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.id;
        buffs = new ItemBuff[item.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].value);
            {
                buffs[i].stats = item.buffs[i].stats;
            };
        }
    }
}

[System.Serializable]
public class ItemBuff
{
    public Stats stats;
    public int value;
    public ItemBuff(int _value)
    {
        value = _value;
    }

}
