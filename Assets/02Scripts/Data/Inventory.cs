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

    /// <summary>
    /// 특정 아이템의 수량을 업데이트 (증가 또는 감소)
    /// </summary>
    /// <param name="itemData">업데이트할 아이템</param>
    /// <param name="amount">변경할 수량 (양수: 증가, 음수: 감소)</param>
    public void UpdateItemQuantity(ItemData itemData, int amount)
    {
        var existingItem = items.FirstOrDefault(i => i.itemData == itemData);

        if (existingItem != null)
        {
            existingItem.quantity += amount;

            // 수량이 0 이하가 되면 아이템 제거
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
                Debug.Log($"[Inventory] {itemData.itemName} 수량이 0이 되어 인벤토리에서 제거됨");
            }
            else
            {
                Debug.Log($"[Inventory] {itemData.itemName} 수량 업데이트: {existingItem.quantity}");
            }
        }
        else if (amount > 0)
        {
            // 아이템이 존재하지 않는데 양수 amount라면 새로 추가
            items.Add(new InventoryItem { itemData = itemData, quantity = amount });
            Debug.Log($"[Inventory] {itemData.itemName} 새로 추가됨: {amount}개");
        }
        else
        {
            Debug.LogWarning($"[Inventory] {itemData.itemName}이 인벤토리에 없어서 수량을 감소시킬 수 없습니다.");
        }
    }

}