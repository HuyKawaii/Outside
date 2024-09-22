using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : InventoryManager
{
    public override void Awake()
    {
        indexOffset = 0;
        inventorySize = 36;
        base.Awake();
    }

    public void Start()
    {
        if (!IsOwner)
            return;

        PlayerData playerData = GameManager.Instance.GetLocalPlayerData();
        if (!playerData.IsEmpty())
        {
            Debug.Log("Has player data");
            int[] inventoryData = playerData.playerInventory;
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventoryData[i * 2] >= 0)
                {
                    Item item = new Item(MultiplayerManager.Instance.GetItemFromIndex(inventoryData[i * 2]), inventoryData[i * 2 + 1]);
                    Add(item, i);
                }
            }
        }
        else
        {
            Debug.Log("Empty player data");
        }
       
    }

    [Command]
    public void GiveItem(int itemIndex, int count)
    {
        Add(new Item(MultiplayerManager.Instance.GetItemFromIndex((int)itemIndex), count));
    }
}

