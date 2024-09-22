using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/New Consumable")]
[Serializable]
public class Consumable : ItemSO
{
    public override bool Use(InventoryManager inventoryManager)
    {
        return base.Use(inventoryManager);
    }
}
