using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeData", menuName = "SO/CoffeeData")]
public class CoffeeData : ScriptableObject
{
    [SerializeField]
    private string coffeeName;
    public string CoffeeName { get { return coffeeName; } }
    [SerializeField]
    private int price;
    public int Price { get { return price; } }
    [SerializeField]
    private int mugQty;
    public int MugQty { get { return mugQty; } }
    [SerializeField]
    private int beanUse;
    public int BeanUse { get { return beanUse; } }
    [SerializeField]
    private Sprite menuIcon;
    public Sprite MenuIcon { get { return menuIcon; } }
}
