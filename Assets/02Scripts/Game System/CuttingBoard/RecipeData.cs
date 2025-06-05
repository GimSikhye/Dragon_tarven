using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public Sprite icon;

    [System.Serializable]
    public class Ingredient
    {
        public MaterialItemData item; // 요구 재료
        public int requiredAmount = 1;
    }

    public List<Ingredient> ingredients = new List<Ingredient>();
    public ItemData outputItem; // 완성된 요리 아이템
}
