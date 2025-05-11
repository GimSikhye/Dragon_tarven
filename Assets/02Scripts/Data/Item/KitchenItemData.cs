using UnityEngine;


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

[CreateAssetMenu(menuName = "SO/Inventory/KitchenItem")]
public class KitchenItemData : ItemData
{
    public KitchenType kitchenType; // SO에서 설정

    public override ItemCategory Category => ItemCategory.Kitchen;

    public override System.Enum SubCategory => kitchenType; 
}
