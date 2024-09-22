using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSlot : InventorySlot
{
    public CraftingManager craftingManager;

    public override void AddItem(Item newItem)
    {
        craftingManager.AddItem(newItem);
        base.AddItem(newItem);
    }

    public override Item RemoveItem()
    {
        craftingManager.RemoveItem(this.item);
        return base.RemoveItem();
    }
}
