using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingWindow : MonoBehaviour
{
    public CraftingRecipeUI[] recipeUIs;

    public static CraftingWindow instance;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        Inventory.instance.onOpenInventory.AddListener(OnOpenIventory);    
    }

    void OnDisable()
    {
        Inventory.instance.onCloseInventory.AddListener(OnOpenIventory);
    }

    void OnOpenIventory()
    {
        gameObject.SetActive(false);
    }

    public void Craft(CraftingRecipe recipe) 
    {
        for (int i = 0; i < recipe.cost.Length; i++) {
            for (int x = 0; x < recipe.cost[i].quantity; x++) {
                Inventory.instance.RemoveItem(recipe.cost[i].item);
            }
        }

        Inventory.instance.AddItem(recipe.itemToCraft);

        for (int i = 0; i< recipeUIs.Length; i++) {
            recipeUIs[i].UpdateCanCraft();
        }
    }
}
