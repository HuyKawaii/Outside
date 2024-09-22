using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentButton : InterativeButton
{
    public EquipmentSlot equipmentSlot;

    public override void ActionOnClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            equipmentSlot.Unequip();
    }

    public override void ActionOnDrop(PointerEventData eventData)
    {
        if (eventData != null)
        {
            InventoryButton targetButton = eventData.pointerDrag.GetComponent<InventoryButton>();
            if (targetButton != null)
                equipmentSlot.SwapEquipment(targetButton.inventorySlot);
        }
    }

    public override void ActionOnMouseEnter(PointerEventData eventData)
    {
    }

    public override void ActionOnMouseExit(PointerEventData eventData)
    {
    }
}
