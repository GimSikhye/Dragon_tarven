using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;

    public ItemData GetItemById(string id)
    {
        return items.Find(x => x.itemName == id);
    }
}
