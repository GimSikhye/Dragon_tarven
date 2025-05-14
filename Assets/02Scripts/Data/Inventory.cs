using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoSingleton<Inventory>
{
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>(); // 아이템 데이터와 그 아이템 수량

    public List<InventoryItem> GetItemsByCategory(ItemCategory category) // 카테고리가 같은 아이템 리스트를 반환함
    {
        return items.Where(i => i.itemData.Category == category).ToList();
    }
    public void AddItem(ItemData itemData, int amount = 1)
    {
        var existingItem = items.FirstOrDefault(i => i.itemData == itemData); // 존재하는 아이템인지 확인

        if (existingItem != null)
        {
            existingItem.quantity += amount;
        }
        else
        {
            items.Add(new InventoryItem { itemData = itemData, quantity = amount }); // 아이템 리스트에 넣어줌
        }
    }

}