using UnityEngine;
public enum CurrencyType { Coin, Gem, CoffeeBean }

[CreateAssetMenu(menuName = "SO/Store/Item")]
public class StoreItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public CurrencyType currency;
    public ItemCategory category;
    public System.Enum subCategory;
    public ItemData itemData; // �κ��丮�� ���� ���� ������ ������
}
