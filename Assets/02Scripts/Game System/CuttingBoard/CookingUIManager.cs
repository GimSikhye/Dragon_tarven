using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject cookingUI;
    [SerializeField] private Transform recipeSlotParent;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private List<RecipeData> recipes;

    public void ToggleCookingUI()
    {
        if(cookingUI.activeSelf)
        {
            cookingUI.SetActive(false);
        }
        else
        {
            RefreshUI();
            cookingUI.SetActive(true);
        }
    }

    private void RefreshUI()
    {
        foreach (Transform child in recipeSlotParent)
            Destroy(child.gameObject);

        foreach(var recipe in recipes)
        {
            GameObject slot = Instantiate(recipeSlotPrefab, recipeSlotParent);
            CookingSlot slotScript = slot.GetComponent<CookingSlot>();
            bool canCook = HasAllIngredients(recipe);
            slotScript.SetRecipe(recipe, canCook); // slot 스크립트 내부
        }
    }

    private bool HasAllIngredients(RecipeData recipe)
    {
        foreach(var ingredient in recipe.ingredients)
        {
            int inventoryAmount = Inventory.Instance
                .GetItemsByCategory(ItemCategory.Material) // Inventory.cs Items List의 InventoryItem: i
                .Where(i => i.itemData == ingredient.item)
                .Sum(i => i.quantity); // .sum을 하는 이유?

            if (inventoryAmount < ingredient.requiredAmount)
                return false;
        }
        return true;
    }

    public void TryCook(RecipeData recipe)
    {
        if (!HasAllIngredients(recipe)) return;

        foreach(var ingredient in recipe.ingredients)
        {
            Inventory.Instance.RemoveItemAmount(ingredient.item, ingredient.requiredAmount);
        }

        Inventory.Instance.AddItem(recipe.outputItem, 1);
    }
}
