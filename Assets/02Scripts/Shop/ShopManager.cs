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
        if (category == ShopCategoryType.Decoration && sub.HasValue) // 아이템들 중 카테고리가 Deco이고, 서브카테고리 인자가 null이 아닐 때
        {
            return allItems.Where(i => i.category == category && i.subCategory == sub.Value).ToList(); // ShopItemData의 요소의 카테고리가(인수의 카테고리와 같고, 서브카테고리가 인자와 같은것을 List로 반환 
        }
        return allItems.Where(i => i.category == category).ToList(); // 서브 카테고리가 null 이거나, ShopCategoryType이 Deco가 아니라면, 인자와 같은 카테고리를 가진 아이템들의 리스트를 반환
    }

    private void Awake()
    {
        Instance = this;
    }
}
