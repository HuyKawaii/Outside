using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public ItemSO itemSO;
    public int count;

    public delegate void OnStackUpdate();
    public OnStackUpdate StackUpdateCallback;
    public delegate void ItemRemoveCallback(Item item);
    public ItemRemoveCallback OnItemRemoveCallback;

    public Item(Item item)
    {
        this.itemSO = item.itemSO;
        this.count = item.count;
    }

    public Item(ItemSO item, int count = 1)
    {
        this.itemSO = item;
        this.count = count;
    }

    public void Use(InventoryManager inventoryManager)
    {
        if (itemSO.Use(inventoryManager))
        {
            if (itemSO.consumeOnUse)
            {
                UpdateStack(-1);
            }
        }
    }

    public void RemoveFromInventory()
    {
        Debug.Log("Removing");
        if (OnItemRemoveCallback != null)
            OnItemRemoveCallback(this);
    }

    public void DropFromInventory(InventoryManager inventory)
    {
        RemoveFromInventory();
        DropInWorld(inventory.transform.position);
    }

    public void DropInWorld(Vector3 position)
    {
        MultiplayerManager.Instance.DropItemInWorld(position, this);
    }

    public virtual bool ReturnToPlayerInventory()
    {
        RemoveFromInventory();
        InventoryManager inventory = PlayerManager.Instance.player.GetComponentInChildren<InventoryManager>();
        return inventory.Add(this);
    }

    public int UpdateStack(int changeInStack)
    {
        Debug.Log("Updating stack");
        count += changeInStack;
        if (StackUpdateCallback != null)
            StackUpdateCallback();
        if (count > itemSO.STACKLIMIT)
            count = itemSO.STACKLIMIT;
        if (count <= 0)
            RemoveFromInventory();
        return count - itemSO.STACKLIMIT;
    }
}
