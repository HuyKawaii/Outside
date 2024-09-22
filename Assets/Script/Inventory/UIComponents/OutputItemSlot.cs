using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputItemSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private Image backGround;
    public CraftingRecipe recipe { get; private set; }
    private Color32 selectedColor = new Color32(100, 100, 100, 255);
    public CraftingManager craftingManager;

    private void Start()
    {
        icon.sprite = recipe.outputItem.item.sprite;
        button.onClick.AddListener(GetSelect);
        button.GetComponent<OutputItemButton>().text = SetRecipeText();
    }

    public void SetRecipe(CraftingRecipe recipe)
    {
        this.recipe = recipe;
        if (recipe.outputItem != null)
            SetRecipeText();
    }

    private string SetRecipeText()
    {
        string text = string.Empty;
        foreach (SerializableItem recipeItem in recipe.inputItemList)   
        {
            text += recipeItem.item.name + " X " + recipeItem.count + "\n";
        }
        return text;
    }

    private void SelectOutputRecipeUI()
    {
        craftingManager.SelectOutputRecipe(recipe);
    }
    
    public void GetSelect()
    {
        backGround.color = selectedColor;
        SelectOutputRecipeUI();
    }

    public void GetUnselect()
    {
        backGround.color = Color.white;
    }

 
}
