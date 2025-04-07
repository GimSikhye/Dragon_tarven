using DalbitCafe.UI;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "SO/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int coffeeBean = 0;
    public int coin = 0;
    public int gem = 0;
    public int exp = 0;


    public int defaultGold = 1500;
    public int defaultGem = 0;
    public int defaultCoffeeBeans = 200;
    public int defaultExp = 0;

    public void AddCoin(int amount)
    {
        coin += amount;
        PlayerPrefs.SetInt("Coin", coin);
        UIManager.Instance.UpdateCoinUI(coin);

    }

    public void AddGem(int amount)
    {
        gem += amount;
        PlayerPrefs.SetInt("Gem", gem);
        UIManager.Instance.UpdateGemUI(gem);

    }

    public void AddCoffeeBean(int amount)
    {
        coffeeBean += amount;
        PlayerPrefs.SetInt("CoffeeBean", coffeeBean);
        UIManager.Instance.UpdateCoffeeBeanUI(coffeeBean);
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

        UIManager.Instance.UpdateCoffeeBeanUI(coin);
        UIManager.Instance.UpdateCoffeeBeanUI(gem);
        UIManager.Instance.UpdateCoffeeBeanUI(coffeeBean);


    }
}
