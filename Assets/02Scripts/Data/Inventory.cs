using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoSingleton<Inventory>
{
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>(); // ������ �����Ϳ� �� ������ ����

    public List<InventoryItem> GetItemsByCategory(ItemCategory category) // ī�װ��� ���� ������ ����Ʈ�� ��ȯ��
    {
        return items.Where(i => i.itemData.Category == category).ToList();
    }
    public void AddItem(ItemData itemData, int amount = 1)
    {
        var existingItem = items.FirstOrDefault(i => i.itemData == itemData); // �����ϴ� ���������� Ȯ��

        if (existingItem != null)
        {
            existingItem.quantity += amount;
        }
        else
        {
            items.Add(new InventoryItem { itemData = itemData, quantity = amount }); // ������ ����Ʈ�� �־���
        }
    }

}