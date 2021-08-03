using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStat Item", menuName = "Inventory System/Items/PlayerStat")]
public class PlayerStatObject : ItemObject
{
    public float restoreHealthValue;
    private void Awake()
    {
        type = ItemType.PlayerStat;
    }
}
