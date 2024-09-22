using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactalble
{
    public InventoryManager chestInventory;
    public ResourceCombat resourceCombat;

    protected override void Start()
    {
        base.Start();
        chestInventory.indexOffset = PlayerManager.Instance.player.GetComponentInChildren<InventoryManager>().inventorySize;
        resourceCombat = GetComponent<ResourceCombat>();  
        resourceCombat.onResourceDeathCallback += DropAllItemInChest;
    }

    private void DropAllItemInChest()
    {
        if (chestInventory != null)
        {
            foreach (Item item in chestInventory.itemList)
            {
                if (item != null)
                {
                    item.DropFromInventory(chestInventory);
                }
            }
        }
    }

    protected override void SetHoverText()
    {
        hoverText = "Press E to open";
    }

    public override void Interact(Transform player)
    {
        Debug.Log("Opened Chest");
        InventoryUI.instance.ToggleChestInventory(this.chestInventory);
    }
}
