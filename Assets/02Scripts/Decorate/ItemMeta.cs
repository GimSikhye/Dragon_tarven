using UnityEngine;
using DalbitCafe.Deco;

public class ItemMeta : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public System.Enum SubCategory => itemData?.SubCategory;

    public ItemCategory Category => itemData != null ? itemData.Category : ItemCategory.Interior;
    public ItemData GetItemData() => itemData;
}
