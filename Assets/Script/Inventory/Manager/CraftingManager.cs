using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class CraftingManager : MonoBehaviour
{
    private Dictionary<ItemSO, Item> itemInGrid;
    public CraftingRecipe outputRecipe { private set; get; }
    public List<CraftingRecipe> recipeList;

    private InventoryManager playerInventory;
    private InventoryManager craftingInventory;

    public delegate void OnOutputItemChange();
    public OnOutputItemChange outputItemChangeCallback;
    public delegate void OnItemInGridChange();
    public OnItemInGridChange inputItemChangeCallback;
    private void Awake()
    {
        craftingInventory = GetComponent<InventoryManager>();
        itemInGrid = new Dictionary<ItemSO, Item>();
    }

    private void Start()
    {
        playerInventory = PlayerManager.Instance.player.GetComponent<InventoryManager>();
    }

    public void AddItem(Item item)
    {
        if (item != null)
        {
            itemInGrid[item.itemSO] = item;
            //Debug.Log(item.name.ToString() + " x " + itemInGrid[item.name].count + " added to crafting grid");
        }
        if (inputItemChangeCallback != null)
            inputItemChangeCallback();
    }

    public void RemoveItem(Item item)
    {
        if (item != null)
        {
            itemInGrid.Remove(item.itemSO);
            //Debug.Log(item.name.ToString() + " removed from crafting grid");
        }
        if (inputItemChangeCallback != null)
            inputItemChangeCallback();
    }

    public bool TryCraftRecipe(CraftingRecipe recipe)
    {
        if (recipe == null)
            return false;
        foreach(SerializableItem recipeItem in recipe.inputItemList)
        {
            if (!itemInGrid.ContainsKey(recipeItem.item) || itemInGrid[recipeItem.item].count < recipeItem.count)
                return false;
        }
        return true;
    }

    public Item CraftOutputItem()
    {
        List<Item> toRemove = new List<Item>();

        if (TryCraftRecipe(outputRecipe))
        {
            foreach (SerializableItem recipeItem in outputRecipe.inputItemList)
            {
                if (itemInGrid.ContainsKey(recipeItem.item))
                    itemInGrid[recipeItem.item].UpdateStack(-recipeItem.count);
                //if (itemInGrid[recipeItem.item].count <= 0)
                //    toRemove.Add(itemInGrid[recipeItem.item]);
            }


            //foreach (Item itemToRemove in toRemove)
            //    itemToRemove.RemoveFromInventory();

            //Debug.Log(outputItem.name.ToString() + " crafted");
            Item output = new Item(outputRecipe.outputItem.item, outputRecipe.outputItem.count);
            return output;
        }
        else
        {
            //Debug.Log(outputItem.name.ToString() + " cant not be crafted");
            return null;
        }
    }

    public void SelectOutputRecipe(CraftingRecipe recipe)
    {
        outputRecipe = recipe;
        //Debug.Log(item.name.ToString() + " selected for crafting");
        if (outputItemChangeCallback != null)
            outputItemChangeCallback();
    }

    public void CraftAndAdd()
    {
        playerInventory.Add(CraftOutputItem());
        if (inputItemChangeCallback != null)
            inputItemChangeCallback();
    }

    public void ReturnItemToInventory()
    {
        for (int i = 0; i < craftingInventory.inventorySize; i++)
        {
            Item itemToRemove = craftingInventory.itemList[i];
            if (itemToRemove != null)
                itemToRemove.ReturnToPlayerInventory();
        }
    }

    public void RemoveOutoutItem()
    {
        outputRecipe = null;
    }
}
