using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : Interactalble
{
    [SerializeField] private SerializableItem item;
    float offSet = 1.0f;

    public override void Interact(Transform player)
    {
        Item newItem = new Item(item.item, item.count);
        newItem.DropInWorld(this.transform.position);
    }

    protected override void SetHoverText()
    {
        if (item != null)
        {
            hoverText = "Generate " + item.item.name;
        }
    }

    protected override void SnapToGround()
    {
    }
}
