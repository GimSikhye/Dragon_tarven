using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingUIManager : MonoBehaviour
{
    // 활성화된 레시피 아이템 클릭시 요리 되고, 코인얻도록 만들기.
    [SerializeField] private GameObject cookingWindow;
    [SerializeField] private Transform recipeSlotParent;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private List<RecipeData> recipes; // recipe SO List

    public void ToggleCookingUI()
    {
        if(cookingWindow.activeSelf)
        {
            cookingWindow.SetActive(false);
        }
        else
        {
            RefreshUI();
            cookingWindow.SetActive(true);
        }
    }

    private void RefreshUI()
    {
        foreach (Transform child in recipeSlotParent)
            Destroy(child.gameObject);

        foreach(var recipe in recipes)
        {
            GameObject recipeSlot = Instantiate(recipeSlotPrefab, recipeSlotParent); 
            RecipeSlot recipeSlotScript = recipeSlot.GetComponent<RecipeSlot>();
            bool canCook = HasAllIngredients(recipe); // recipe의 재료가 전부 있으면 canCook(true)
            recipeSlotScript.SetRecipe(recipe, canCook); // canCook 여부에 따라서 display
        }
    }

    private bool HasAllIngredients(RecipeData recipe) // 해당 레시피 데이터의 모든 재료를 가지고 있으면 true ((( 여기부터 읽기))))
    {
        foreach(var ingredient in recipe.ingredients)
        {
            int inventoryAmount = Inventory.Instance
                .GetItemsByCategory(ItemCategory.Material) // Inventory.cs Items List의 InventoryItem: i
                .Where(i => i.itemData == ingredient.item)
                .Sum(i => i.quantity); // Sum: 컬렉션 안의 값들을 모두 더해서 합계를 구해주는 함수 // 해당 아이템이 인벤토리에 여러 개 있을 수 있으므로, 그 총 수량(quantity)을 전부 더함

            if (inventoryAmount < ingredient.requiredAmount)
                return false;
        }
        return true;
    }

    public void TryCook(RecipeData recipe) // 지금 재료 수량이 안줄어드는중
    {
        if (!HasAllIngredients(recipe)) return;

        foreach(var ingredient in recipe.ingredients)
        {
            Inventory.Instance.RemoveItemAmount(ingredient.item, ingredient.requiredAmount);
        }

        Inventory.Instance.AddItem(recipe.outputItem, 1);
    }
}
