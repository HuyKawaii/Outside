using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PickupItemBehaviour : Interactalble
{
    public SerializableItem item;
    public InventoryManager playerInventory;

    protected override void SetHoverText()
    {
        hoverText = "Press E to pick up";
    }
    public override void Interact(Transform player)
    {
        //Debug.Log(player.name);
        playerInventory = player.GetComponentInChildren<InventoryManager>();
        //Debug.Log("Pick up");
        if (playerInventory.Add(new Item(item.item, item.count)))
            DestroyAfterPickupServerRpc();
        else
            Debug.Log("Inventory Full");
    }

    [Rpc(SendTo.Server)]
    private void DestroyAfterPickupServerRpc()
    {
        Destroy(gameObject);
    }

}
