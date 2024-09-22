using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class InterativeButton : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform itemSlot;
    private Canvas inventoryCanvas;
    private CanvasGroup canvasGroup;

    protected bool isMouseOverButtom;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryCanvas = GameObject.Find("InventoryCanvas").GetComponent<Canvas>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ActionOnClick(eventData);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverButtom = true;
        ActionOnMouseEnter(eventData);
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverButtom = false;
        ActionOnMouseExit(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemSlot.anchoredPosition += eventData.delta / inventoryCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        itemSlot.anchoredPosition = Vector2.zero;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ActionOnDrop(eventData);
    }

    public abstract void ActionOnDrop(PointerEventData eventData);
    public abstract void ActionOnClick(PointerEventData eventData);
    public abstract void ActionOnMouseEnter(PointerEventData eventData);
    public abstract void ActionOnMouseExit(PointerEventData eventData);


}
