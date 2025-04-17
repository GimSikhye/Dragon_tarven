using DalbitCafe.Operations;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public PlayerStats statsSO;

    public int CoffeeBeans { get; private set; }
    public int Coin { get; private set; }
    public int Gem { get; private set; }
    public int Exp { get; private set; }
    public int Level { get; private set; }
    public int MaxExp { get; private set; }

    void Awake()
    {
        InitializeStat("Coin", statsSO.coin);
        InitializeStat("Gem", statsSO.gem);
        InitializeStat("CoffeeBean", statsSO.coffeeBean);
        InitializeStat("Exp", statsSO.exp);
        InitializeStat("Level", statsSO.level);
    }

    void InitializeStat(string key, int defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, defaultValue);
        }
    }

    public void Load()
    {
        CoffeeBeans = PlayerPrefs.GetInt("CoffeeBean");
        Coin = PlayerPrefs.GetInt("Coin");
        Gem = PlayerPrefs.GetInt("Gem");
        Exp = PlayerPrefs.GetInt("Exp");
        Level = PlayerPrefs.GetInt("Level");
        MaxExp = CalculateMaxExp(Level);
    }

    public void AddCoin(int amount)
    {
        Coin += amount;
        PlayerPrefs.SetInt("Coin", Coin);
        GameManager.Instance.UIManager.UpdateCoinUI(Coin);
    }

    public void AddGem(int amount)
    {
        Gem += amount;
        PlayerPrefs.SetInt("Gem", Gem);
        GameManager.Instance.UIManager.UpdateGemUI(Gem);
    }

    public void AddCoffeeBean(int amount)
    {
        CoffeeBeans += amount;
        PlayerPrefs.SetInt("CoffeeBean", CoffeeBeans);
        GameManager.Instance.UIManager.UpdateCoffeeBeanUI(CoffeeBeans);
    }

    public void AddExp(int amount)
    {
        Exp += amount;

        while (Exp >= MaxExp)
        {
            Exp -= MaxExp;
            Level++;
            MaxExp = CalculateMaxExp(Level);
        }

        PlayerPrefs.SetInt("Exp", Exp);
        PlayerPrefs.SetInt("Level", Level);

        GameManager.Instance.UIManager.UpdateExpUI(Exp, MaxExp, Level);
    }

    private int CalculateMaxExp(int currentLevel)
    {
        return 100 + (currentLevel - 1) * 20;
    }

    // 예시: PlayerStatsManager 안에 레벨업 함수 내에서
    public void LevelUp()
    {
        Level++; // 레벨 증가

        // 기타 레벨업 처리...

        GameManager.Instance.CoffeeMachineManager.UpdateMachineActivation(Level);
    }

}
