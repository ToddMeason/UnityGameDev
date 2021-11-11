using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public delegate void PickedUpItem();
    public static event PickedUpItem pickedUpItem;

    public string savePath;
    private ItemDatabaseObject database {get { return database;} set { database = value; } }//May need to be private and reworked so it doesnt get overwritten and could be problems when buidling
    public Inventory Container;
    public List<InventorySlot> GetSlots { get { return Container.Slots; } }

    private void OnEnable()
    {
        database = Resources.Load("Database") as ItemDatabaseObject;
        Debug.Log(database);
    }

    public void AddItem(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Slots.Count; i++)//Check if item is in inventory
        {
            if (Container.Slots[i].item.Id == _item.Id)
            {
                Container.Slots[i].AddAmount(_amount);
                pickedUpItem?.Invoke();
                return;
            }
        }
        Container.Slots.Add(new InventorySlot(_item.Id, _item, _amount));

       pickedUpItem?.Invoke();
    }

    #region Save/Load
    [ContextMenu("Save")]
    public void Save()//save path is C:/Users/'username'/AppData/LocalLow/DefaultCompany/Rover
    {
        Debug.Log("Saving");

        //Could use IFormatter to make file uneditable
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + savePath);
        bf.Serialize(file, saveData);
        file.Close();
        Debug.Log(Application.persistentDataPath);
    }

    [ContextMenu("Load")]
    public void Load()
    {       
        if (File.Exists(Application.persistentDataPath + savePath))
        {
            Debug.Log("Load Inv");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + savePath, FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
        else
        {
            Debug.Log("not Load Inv");
        }
        pickedUpItem?.Invoke();//just to call update inventoryDisplay
    }

    [ContextMenu("Clear Inventory")]
    public void Clear()
    {
        Container = new Inventory();
    }
    #endregion
}

[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Slots = new List<InventorySlot>();
}

[System.Serializable]
public class InventorySlot
{
    [System.NonSerialized]
    public GameObject slot;

    public int ID;
    public Item item;
    public int amount;
    public InventorySlot(int _ID, Item _item, int _amount)
    {
        ID = _ID;
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
