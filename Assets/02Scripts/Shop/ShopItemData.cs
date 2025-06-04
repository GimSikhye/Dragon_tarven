using UnityEngine;

[CreateAssetMenu(menuName = "SO/Shop/Item")]
public class ShopItemData : ScriptableObject
{
    public string itemName; // 아이템 이름
    [TextArea] public string description; // 아이템 설명 (TextArea 속성을 통해 인스펙터에서 줄바꿈이 가능한 입력 필드 제공
    public Sprite icon; // 아이템 아이콘
    public int price; // 아이템 가격
    public ShopCategoryType category; // 카테고리
    public DecoSubCategory subCategory; //  Decoration 아이템에만 해당되는 서브 카테고리

    public ItemData itemData; //아이템 데이터
}
