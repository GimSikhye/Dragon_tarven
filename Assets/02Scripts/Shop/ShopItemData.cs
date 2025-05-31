using UnityEngine;

[CreateAssetMenu(menuName = "SO/Shop/Item")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public int price;
    public ShopCategoryType category;
    public DecoSubCategory subCategory;

    public ItemData itemData;
}
