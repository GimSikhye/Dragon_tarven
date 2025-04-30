using UnityEngine;

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

[CreateAssetMenu(menuName = "SO/Inventory/InteriorItem")]
public class InteriorItemData : ItemData
{
    public InteriorType interiorType;

    public override ItemCategory Category => ItemCategory.Interior;
    public override System.Enum SubCategory => interiorType;
}
