using UnityEngine;
public enum ItemCategory
{
    Kitchen, // 주방
    Interior, // 인테리어
    Exterior // 익스테리어
}

public enum KitchenType
{
    RoastingMachine, // 로스팅머신
    CoffeeMachine, // 커피머신
    Workbench, // 작업대
    CookingMachine, // 쿠킹머신
    Showcase, // 쇼케이스
    Counter, // 계산대
    Mixer // 믹서기
}

public enum InteriorType
{
    Table, // 테이블
    Chair, // 의자
    Partition, // 파티션
    Decoration, // 장식품 
    BeanContainer, // 원두통
    WallDecoration, // 벽장식
    Tile, // 타일
    Wallpaper // 벽지
}

public enum ExteriorType
{
    SecondFloorOnly, // 2층전용
    OutdoorDecoration, // 야외장식품
    WallExteriorDecoration, // 건물 외벽 장식
    Railing2F, // 2층 난간
    Stair2F, // 2층 계단
    WallExterior, // 건물 외벽
    Entrance // 입구
}

[CreateAssetMenu(menuName = "SO/Inventory/ItemData")]
public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public GameObject prefab; // 배치용 프리팹

    public abstract ItemCategory Category { get; }

}
