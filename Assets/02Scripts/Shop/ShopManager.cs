using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Shop Items")]
    [SerializeField] private List<ShopItemData> allItems;

    public List<ShopItemData> GetItems(ShopCategoryType category, DecoSubCategory? sub = null)
    {
        if (category == ShopCategoryType.Decoration && sub.HasValue)
        {
            return allItems.Where(i => i.category == category && i.subCategory == sub.Value).ToList();
        }
        return allItems.Where(i => i.category == category).ToList();
    }

    private void Awake()
    {
        Instance = this;
    }
}
