using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventoryManager : NetworkBehaviour
{
    public int indexOffset = 0;
    public List<Item> itemList;
    public int inventorySize;
    public delegate void OnInventoryUpdate(Item item, int index);
    public OnInventoryUpdate inventoryAddCallBack;
    public OnInventoryUpdate inventoryRemoveCallBack;

    public virtual void Awake()
    {
        itemList = new List<Item>();
        for (int i = 0; i < inventorySize; i++)
        {
            itemList.Add(null);
        }
    }
    public bool Add(Item item)
    {
        if (item == null)
            return false;

        if (item.itemSO.isStackable)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (itemList[i] != null && item.itemSO == itemList[i].itemSO && itemList[i].count < item.itemSO.STACKLIMIT)
                {
                    return Add(item, i + indexOffset);
                }
            }
        }
        
        for (int i = 0; i < inventorySize; i++)
            if (itemList[i] == null)
            {
                //Debug.Log("Found empty slot");
                return Add(item, i + indexOffset);
            }

        return false;
        
    }

    private void AddItemToNext(Item item)
    {

    }

    public bool Add(Item item, int index)
    {
        //Debug.Log(this.gameObject.name);
        if (item == null)
        {
            //Debug.Log("Null item");
            return false;
        }

        if (index < inventorySize + indexOffset && index >= indexOffset)
        {
            //Debug.Log("Add to slot number " + index);

            if (itemList[index - indexOffset] != null && itemList[index - indexOffset].itemSO == item.itemSO)
            {
                item.UpdateStack(itemList[index].count);
            }

            itemList[index - indexOffset] = item;
            if (GetComponent<NetworkObject>() != null)
            {
                Debug.Log("Sending RPC to server");
                UpdateInventoryServerRpc(MultiplayerManager.Instance.GetIndexFromItem(item), item.count, index - indexOffset);
            }
            item.OnItemRemoveCallback += Remove;
            if (inventoryAddCallBack != null)
                inventoryAddCallBack.Invoke(item, index);
            return true;
        }
        return false;
    }

    public virtual void Remove(Item item)
    {
        if (item == null)
        {
            //Debug.Log("Null item");
            return;
        }

        int itemIndex = itemList.IndexOf(item);
        if (itemIndex >= 0 && itemIndex < inventorySize)
        {
            itemList[itemIndex] = null;
            if (GetComponent<NetworkObject>() != null)
                UpdateInventoryServerRpc(-1, 0, itemIndex);
        }
        //Debug.Log(itemIndex);
        item.OnItemRemoveCallback -= Remove;
        if (inventoryRemoveCallBack != null)
            inventoryRemoveCallBack.Invoke(item, itemIndex + indexOffset);
    }

    [Rpc(SendTo.Server)]
    private void UpdateInventoryServerRpc(int itemIndex, int itemCount, int slotIndex = -1)
    {
        if (!IsLocalPlayer)
        {
            Item newItem;

            if (itemIndex < 0)
                newItem = null;
            else
                newItem = new Item(MultiplayerManager.Instance.GetItemFromIndex(itemIndex), itemCount);

            itemList[slotIndex] = newItem;
            if (inventoryAddCallBack != null)
                inventoryAddCallBack.Invoke(newItem, slotIndex + indexOffset);
        }
        UpdateInventoryAllPlayerRpc(itemIndex, itemCount, slotIndex);
    }

    [Rpc(SendTo.NotServer)]
    private void UpdateInventoryAllPlayerRpc(int itemIndex, int itemCount, int slotIndex = -1)
    {
        if (!IsOwner)
        {
            Item newItem;

            if (itemIndex < 0)
                newItem = null;
            else
                newItem = new Item(MultiplayerManager.Instance.GetItemFromIndex(itemIndex), itemCount);

            itemList[slotIndex] = newItem;
            if (inventoryAddCallBack != null)
                inventoryAddCallBack.Invoke(newItem, slotIndex + indexOffset);
        }
    }
}
