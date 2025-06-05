using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoSingleton<Inventory> // �������� �ʿ��ϰ�, ���Ӿ����� �ʿ��ϹǷ�
{
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>(); // ������ �����Ϳ� �� ������ ����(�߻�ȭ Ŭ����)

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

    public void RemoveItem(InventoryItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
        else
        {
            Debug.LogWarning("Iventory���� �����Ϸ��ϴ� �������� �������� ����");
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