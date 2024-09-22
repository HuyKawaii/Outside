using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Item/New Recipe")]
[Serializable]
public class CraftingRecipe : ScriptableObject
{
    public SerializableItem outputItem;

    [SerializeField]
    public List<SerializableItem> inputItemList;
}

[Serializable]
public class SerializableItem
{
    public ItemSO item;
    public int count;

    public SerializableItem(ItemSO item, int count)
    {
        this.item = item;
        this.count = count;
    }
}