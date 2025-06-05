using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoSingleton<Inventory> // 상점에도 필요하고, 게임씬에도 필요하므로
{
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>(); // 아이템 데이터와 그 아이템 수량(추상화 클래스)

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

    public void RemoveItem(InventoryItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
        else
        {
            Debug.LogWarning("Iventory에서 제거하려하는 아이템이 존재하지 않음");
        }
    }

    public void RemoveItemAmount(ItemData itemData, int amount)
    {
        var item = items.FirstOrDefault(i => i.itemData == itemData);
        if ((item != null))
        {
            item.quantity -= amount;
            if (item.quantity <= 0)
                items.Remove(item);
        }

    }

}