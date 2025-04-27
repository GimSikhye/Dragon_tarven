using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>(); 

    public List<InventoryItem> GetItemsByCategory(ItemCategory category) 
    {
        return items.Where(i => i.itemData.Category == category).ToList();
    }
    public void AddItem(ItemData itemData, int amount = 1)
    {
        var existingItem = items.FirstOrDefault(i => i.itemData == itemData);

        if (existingItem != null)
        {
            existingItem.quantity += amount;
        }
        else
        {
            items.Add(new InventoryItem { itemData = itemData, quantity = amount });
        }
    }

}