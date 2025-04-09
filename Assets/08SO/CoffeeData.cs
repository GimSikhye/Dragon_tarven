using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeData", menuName = "SO/CoffeeData")]
public class CoffeeData : ScriptableObject
{
    [SerializeField] private string coffeeId; // °íÀ¯ ID (ex: "americano")
    public string CoffeeId => coffeeId;

    [SerializeField] private string coffeeName;
    public string CoffeeName => coffeeName;

    [SerializeField] private int price;
    public int Price => price;

    [SerializeField] private int mugQty;
    public int MugQty => mugQty;

    [SerializeField] private int beanUse;
    public int BeanUse => beanUse;

    [SerializeField] private Sprite menuIcon;
    public Sprite MenuIcon => menuIcon;
}
