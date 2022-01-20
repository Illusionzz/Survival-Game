using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingRecipeUI : MonoBehaviour
{
    public BuildingRecipe recipe;
    public Image backgroundImage;
    public Image icon;
    public TextMeshProUGUI buildingName;
    public Image[] resourceCost;

    public Color canBuildColor;
    public Color cantBuildColor;
    private bool canBuild;

    void OnEnable()
    {
        UpdateCanCraft();
    }

    void Start()
    {
        icon.sprite = recipe.icon;
        buildingName.text = recipe.displayName;

        for (int x = 0; x < resourceCost.Length; x++) {
            if (x < recipe.cost.Length) {
                resourceCost[x].gameObject.SetActive(true);
                resourceCost[x].sprite = recipe.cost[x].item.icon;
                resourceCost[x].transform.GetComponentInChildren<TextMeshProUGUI>().text = recipe.cost[x].quantity.ToString();
            }
            else {
                resourceCost[x].gameObject.SetActive(false);
            }
        }
    }

    void UpdateCanCraft() 
    {
        canBuild = true;

        for (int x = 0; x < recipe.cost.Length; x++) {
            if (!Inventory.instance.HasItems(recipe.cost[x].item, recipe.cost[x].quantity)) {
                canBuild = false;
                break;
            }
        }
        backgroundImage.color = canBuild ? canBuildColor : cantBuildColor;
    }

    public void OnClickButton() 
    {
        if (canBuild) {
            EquipBuildingKit.instance.SetNewBuildingRecipe(recipe);
        }
        else {
            PlayerController.instance.ToggleCursor(true);
            gameObject.SetActive(false);
        }
    }
}
