using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/New Coin")]
public class Coin : ItemSO
{
    public int value;
    public override bool Use(InventoryManager inventoryManager)
    {
        if (!base.Use(inventoryManager))
            return false;

        PlayerEconManager playerEcon = inventoryManager.transform.GetComponent<PlayerEconManager>();
        if (playerEcon != null)
        {
            playerEcon.GainGold(value);
            Debug.Log("Player gains " + value + " gold");
            //RemoveFromInventory();
            return true;
        }
        else
        {
            Debug.Log("Coin not in player inventory");
            return false;
        }
    }
}
