using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    public int index;
    [NonSerialized] public Item item = null;
    public InventoryManager inventoryManager;
    public TextMeshProUGUI stackCount;

    private void OnEnable()
    {
        ResetUI();
    }
    public virtual void AddItem(Item newItem)
    {
        if (newItem == null)
        {
            RemoveItem();
            return;
        }

        item = newItem;
        item.StackUpdateCallback += ResetUI; //Need change here
        ResetUI();
    }

    public virtual Item RemoveItem()
    {
        Item oldItem = item;
        if (oldItem != null)
            oldItem.StackUpdateCallback -= ResetUI;
        item = null;
        ResetUI();
        return oldItem;
    }

    protected void ResetUI()
    {
        if (item != null)
        {
            icon.sprite = item.itemSO.sprite;
            icon.enabled = true;
            if (item.count > 1)
                stackCount.text = item.count.ToString();
            else
                stackCount.text = default;
        }else
        {
            icon.sprite = null;
            icon.enabled = false;
            stackCount.text = default;
        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use(inventoryManager);
        }
    }

    public void DropItem()
    {
        if (item != null)
        {
            item.DropFromInventory(inventoryManager);
        }
    }

    public void SwapItemSlot(InventorySlot swapTarget)
    {
        Item targetItem = swapTarget.item;
        Item thisItem = item;

        inventoryManager.Remove(thisItem);
        swapTarget.inventoryManager.Remove(targetItem);

        if (targetItem != null && thisItem != null && targetItem.itemSO == thisItem.itemSO && thisItem.itemSO.isStackable)
        {
            thisItem.UpdateStack(targetItem.count);
            inventoryManager.Add(thisItem, index);
        }else
        {
            inventoryManager.Add(targetItem, index);
            swapTarget.inventoryManager.Add(thisItem, swapTarget.index);
        }
    }

}
