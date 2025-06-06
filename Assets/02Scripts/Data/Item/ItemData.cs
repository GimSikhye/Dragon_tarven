using UnityEngine;
public enum ItemCategory
{
    Kitchen, // 주방
    Interior, // 인테리어
    Exterior, // 익스테리어
    Material, // 재료(요리 위함)
    Food
}
// 아이템, 벽, 바닥, 테이블
public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price; // 되파는 가격
    public GameObject prefab; // 배치용 프리팹(데코 아이템일 경우)

    public abstract ItemCategory Category { get; }
    public virtual System.Enum SubCategory => null; // default



}
