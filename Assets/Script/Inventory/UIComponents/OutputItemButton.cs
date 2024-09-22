using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OutputItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUI.instance.toolTip.ShowTooltip(transform.position, text);
        Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.instance.toolTip.HideTooltip();
        Debug.Log("Exit");
    }
}
