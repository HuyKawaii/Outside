using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/New Item")]
[Serializable]
public class ItemSO : ScriptableObject
{
    public int STACKLIMIT = 64;
    public new ItemName name;
    public string description;
    public bool isStackable;

    public Sprite sprite;
    public SkinnedMeshRenderer mesh;

    public GameObject itemPickupPrefab;
    public GameObject itemGraphic;

    public bool consumeOnUse;
    [SerializeReference, SubclassSelector] 
    public List<ItemEffect> effects;

    public Vector3 holdingRotation;
    public Vector3 holdingPosition;

    public virtual bool Use(InventoryManager inventoryManager)
    {
        Debug.Log("Used " + name + " from inventory " + inventoryManager);
        if (inventoryManager != PlayerManager.Instance.player.GetComponent<InventoryManager>())
        {
            Debug.Log("Not using from player inventory");
            return false;
        }
        foreach (ItemEffect itemEffect in effects)
        {
            itemEffect.UseEffect(inventoryManager);
        }
        return true;
    }

    
    [Serializable]
    public enum ItemName { Sword, Shield, Helmet, PlateBody, PlateLegs,
        Iron, Gold,
        PineLog, OakLog, BirchLog,
        WoodenPickaxe, IronPickaxe,
        None, Stick, Peddel,
        WoodenAxe,
        Coin, Diamon,
        GoldPickaxe,
        DiamonPickaxe,
        IronAxe, GoldAxe, DiamonAxe,
        CraftingTable,
        Plank,
        HealingMushroom, Mushroom,
        BrokenBone, BrokenClub, BrokenBow,
        Chest
    };
}

[Serializable]
public abstract class ItemEffect
{
    public abstract void UseEffect(InventoryManager playerInventory);
}

[Serializable]
public class HealEffect : ItemEffect
{
    public float healAmount;
    public override void UseEffect(InventoryManager playerInventory)
    {
        playerInventory.GetComponent<PlayerCombat>().Heal(healAmount);
    }
}

[Serializable]
public class GainCoin : ItemEffect
{
    public int coinAmount;

    public override void UseEffect(InventoryManager playerInventory)
    {
        playerInventory.GetComponent<PlayerEconManager>().GainGold(coinAmount);
    }
}

[Serializable]
public class Restore : ItemEffect
{
    public float nutrientValue;

    public override void UseEffect(InventoryManager playerInventory)
    {
        playerInventory.GetComponent<PlayerController>().RestoreHunger(nutrientValue);
    }
}