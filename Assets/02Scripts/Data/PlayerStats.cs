using DalbitCafe.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "SO/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int coffeeBean = 0;
    public int coin = 0;
    public int gem = 0;
    public int exp = 0;
    public int level = 1;
    public int maxExp = 100;

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

        while (exp >= maxExp)
        {
            exp -= maxExp;
            level++;
            maxExp = CalculateMaxExp(level);
        }

        PlayerPrefs.SetInt("Exp", exp);
        PlayerPrefs.SetInt("Level", level);

        UIManager.Instance.UpdateExpUI(exp, maxExp, level);
    }

    private int CalculateMaxExp(int currentLevel)
    {
        return 100 + (currentLevel - 1) * 20; // 간단한 경험치 곡선
    }

    public void LoadFromPrefs()
    {
        coffeeBean = PlayerPrefs.GetInt("CoffeeBean", 100);
        coin = PlayerPrefs.GetInt("Coin", 1000);
        gem = PlayerPrefs.GetInt("Gem", 0);
        exp = PlayerPrefs.GetInt("Exp", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        maxExp = CalculateMaxExp(level);

        UIManager.Instance.UpdateCoinUI(coin);
        UIManager.Instance.UpdateGemUI(gem);
        UIManager.Instance.UpdateCoffeeBeanUI(coffeeBean);
        UIManager.Instance.UpdateExpUI(exp, maxExp, level);
    }
}
