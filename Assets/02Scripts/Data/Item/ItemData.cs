using UnityEngine;
public enum ItemCategory
{
    Kitchen, // 주방
    Interior, // 인테리어
    Exterior // 익스테리어
}

// 모든 아이템이 공통으로 가지는 정보들
[CreateAssetMenu(menuName = "SO/Inventory/ItemData")]
public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public GameObject prefab; // 배치용 프리팹

    public abstract ItemCategory Category { get; }
    public virtual System.Enum SubCategory => null;



}
