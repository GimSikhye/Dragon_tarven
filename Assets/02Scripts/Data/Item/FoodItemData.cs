using UnityEngine;
public enum FoodType
{
    Drink,
    SideMenu
}
[CreateAssetMenu(menuName = "SO/Inventory/FoodItemData")]
public class FoodItemData : ItemData
{
    public FoodType foodType;

    public override ItemCategory Category => ItemCategory.Food;
    public override System.Enum SubCategory => foodType;

}
