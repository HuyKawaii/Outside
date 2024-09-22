using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickAssetSlotUI : InventorySlot
{
    private Image backGround;
    private Color32 selectedColor = new Color32(224, 137, 43, 255);
    private Color32 unSelectedColor = new Color32(183, 102, 27, 255);
    private bool isSelected;

    public delegate void OnSlotUpdated(Item item);
    public OnSlotUpdated slotUpdatedCallback;

    private void Awake()
    {
        backGround = GetComponentInChildren<Image>();    
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSelected && !InventoryUI.instance.isInventoryOpen && item != null)
        {
            item.Use(inventoryManager);
        }
    }

    public override void AddItem(Item newItem)
    {
        if (isSelected && slotUpdatedCallback != null)
            slotUpdatedCallback(newItem);
        base.AddItem(newItem);
    }

    public override Item RemoveItem()
    {
        if (isSelected && slotUpdatedCallback != null)
            slotUpdatedCallback(null);
        return base.RemoveItem();
    }

    public void GetSelect()
    {
        slotUpdatedCallback(item);
        backGround.color = selectedColor;
        isSelected = true;
    }

    public void GetUnselect()
    {
        backGround.color = unSelectedColor;
        isSelected = false;
    }
}
