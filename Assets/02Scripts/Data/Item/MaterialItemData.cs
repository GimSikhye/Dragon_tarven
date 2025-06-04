using UnityEngine;

public enum MaterialType
{
    Drink, // 음료(미니게임에 쓰일 재료)
    SideMenu, // 냉장고로 음식 제작할때 (냉장고 UI에 띄울 재료들)
    Both // 둘다 쓰이는 재료들(음료와 사이드메뉴 둘다)
}

[CreateAssetMenu(menuName = "SO/Inventory/MaterialItem")]
public class MaterialItemData : ItemData
{
    public MaterialType materialType;
    public override ItemCategory Category => ItemCategory.Material;
    public override System.Enum SubCategory => materialType;


}
