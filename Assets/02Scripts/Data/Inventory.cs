//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//[System.Serializable]
//public class InventoryItem
//{
//    public ItemData itemData;
//    public int quantity;
//}

//public class Inventory : MonoBehaviour
//{
//    public List<InventoryItem> items = new();

//    public void AddItem(ItemData data, int amount = 1)
//    {
//        var existing = items.Find(i => i.itemData == data);
//        if (existing != null)
//        {
//            existing.quantity += amount;
//        }
//        else
//        {
//            items.Add(new InventoryItem { itemData = data, quantity = amount });
//        }
//    }

//    public List<InventoryItem> GetItemsByCategory(ItemCategory category)
//    {
//        return items.Where(i => i.itemData.category == category).ToList();
//    }

//    public List<InventoryItem> GetItemsByKitchenType(KitchenType type)
//    {
//        return items.Where(i => i.itemData.category == ItemCategory.Kitchen && i.itemData.kitchenType == type).ToList();
//    }

//    // 인테리어, 익스테리어도 동일하게...
//}
