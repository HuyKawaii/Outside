using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : Interactalble
{
    protected override void SetHoverText()
    {
        hoverText = "Press E to craft";
    }

    public override void Interact(Transform player)
    {
        //Debug.Log("Open crafting window");
        InventoryUI.instance.ToggleCraftingPanel();
    }
}
