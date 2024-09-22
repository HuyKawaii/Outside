using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/New Placeable")]
public class Placeable : ItemSO
{
    public GameObject placedFerfab;
    public override bool Use(InventoryManager inventoryManager)
    {
        if (!base.Use(inventoryManager))
            return false;

        PlayerInteract playerInteract = PlayerManager.Instance.player.GetComponentInChildren<PlayerInteract>();
        if (playerInteract.lookingAtTransform != null && playerInteract.lookingAtTransform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            MultiplayerManager.Instance.PlaceObjectInWorldMultiplayer(this, playerInteract.lookingAtHitPosition);
            return true;
        }
       return false;
    }
}
