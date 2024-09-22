using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryButton : InterativeButton
{
    public InventorySlot inventorySlot;

    protected override void Awake()
    {
        base.Awake();
        inventorySlot = transform.parent.GetComponent<InventorySlot>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isMouseOverButtom)
            inventorySlot.DropItem();
    }

    public override void ActionOnClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            inventorySlot.UseItem();
    }

    public override void ActionOnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventoryButton inventoryButton = eventData.pointerDrag.GetComponent<InventoryButton>();
            if (inventoryButton != null)
            {
                //Debug.Log("Drop from " + inventoryButton.transform.parent.parent.name);
                inventorySlot.SwapItemSlot(inventoryButton.inventorySlot);
            }
            else
            {
                Debug.Log("Drop from equipment");
                EquipmentButton equipmentButton = eventData.pointerDrag.GetComponent<EquipmentButton>();
                if (equipmentButton != null)
                {
                    equipmentButton.equipmentSlot.Unequip();
                }

            }
        }
    }

    public override void ActionOnMouseEnter(PointerEventData eventData)
    {
        if (inventorySlot.item != null)
        {
            //Debug.Log("Item: " + inventorySlot.item.itemSO.name.ToString() + '\n' + inventorySlot.item.itemSO.description);
            InventoryUI.instance.toolTip.ShowTooltip(transform.position + GetComponent<RectTransform>().rect.width * Vector3.right/2, inventorySlot.item.itemSO.name.ToString());// + '\n' + inventorySlot.item.itemSO.description);
        }
    }

    public override void ActionOnMouseExit(PointerEventData eventData)
    {
        InventoryUI.instance.toolTip.HideTooltip();
    }

}
