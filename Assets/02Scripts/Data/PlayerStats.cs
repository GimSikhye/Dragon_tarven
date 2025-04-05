using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "SO/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int coffeeBean = 100;
    public int coin = 1000;
    public int gem = 0;
    public int exp = 0;

    public void AddCoin(int amount)
    {
        coin += amount;
        PlayerPrefs.SetInt("Coin", coin);
    }

    public void AddGem(int amount)
    {
        gem += amount;
        PlayerPrefs.SetInt("Gem", gem);
    }

    public void AddCoffeeBean(int amount)
    {
        coffeeBean += amount;
        PlayerPrefs.SetInt("CoffeeBean", coffeeBean);
    }

    public void AddExp(int amount)
    {
        exp += amount;
        PlayerPrefs.SetInt("Exp", exp);
    }

    public void LoadFromPrefs()
    {
        coffeeBean = PlayerPrefs.GetInt("CoffeeBean", 100);
        coin = PlayerPrefs.GetInt("Coin", 1000);
        gem = PlayerPrefs.GetInt("Gem", 0);
        exp = PlayerPrefs.GetInt("Exp", 0);
    }
}
