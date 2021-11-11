using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    public GameObject inventoryPrefab;
    public InventoryObject inventory;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Slots.Count; i++)
        {
            InventorySlot slot = inventory.Container.Slots[i];

            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiIcon;
            obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");

            itemsDisplayed.Add(slot, obj);
        }
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Slots.Count; i++)
        {
            InventorySlot slot = inventory.Container.Slots[i];

            if (itemsDisplayed.ContainsKey(slot))
            {
                itemsDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            }
            else
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                //obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiIcon;
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");

                itemsDisplayed.Add(slot, obj);
            }
        }
   
    }

    #region Events
    private void OnEnable()
    {
        InventoryObject.pickedUpItem += UpdateDisplay;
    }

    private void OnDisable()
    {
        InventoryObject.pickedUpItem -= UpdateDisplay;
    }
    #endregion
}
