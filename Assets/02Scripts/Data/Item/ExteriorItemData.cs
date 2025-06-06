using UnityEngine;

public enum ExteriorType // 익스테리어 타입의 서브카테고리들
{
    SecondFloorOnly, // 2층전용
    OutdoorDecoration, // 야외장식품
    WallExteriorDecoration, // 건물 외벽 장식
    Railing2F, // 2층 난간
    Stair2F, // 2층 계단
    WallExterior, // 건물 외벽
    Entrance // 입구
}

[CreateAssetMenu(menuName = "SO/Inventory/ExteriorItem")]
public class ExteriorItemData : ItemData
{
    public ExteriorType exteriorType; // 세부 타입
    public override ItemCategory Category => ItemCategory.Exterior;
    public override System.Enum SubCategory => exteriorType;

}
