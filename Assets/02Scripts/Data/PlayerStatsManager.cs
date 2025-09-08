using DalbitCafe.Operations;
using UnityEngine;

public class PlayerStatsManager : MonoSingleton<PlayerStatsManager>
{
    public PlayerStats statsSO;
    private UIManager uiManager;
    public int CoffeeBeans { get; private set; }
    public int Coin { get; private set; }
    public int Gem { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        //InitializeStat("Coin", statsSO.coin);
        //InitializeStat("Coin", 100); // 일단 테스트로 항시 값 바꾸기
        //InitializeStat("Gem", 10);
        //InitializeStat("CoffeeBean", 1000);

        LoadStat();
    }


    private void InitializeStat(string key, int defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, defaultValue);
        }
    }

    public void LoadStat()
    {
        if (uiManager == null) uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        CoffeeBeans = PlayerPrefs.GetInt("CoffeeBean");
        Coin = PlayerPrefs.GetInt("Coin");
        Gem = PlayerPrefs.GetInt("Gem");

        uiManager.UpdateCoinUI(Coin);
        uiManager.UpdateGemUI(Gem);
        uiManager.UpdateCoffeeBeanUI(CoffeeBeans);

    }

    public void AddCoin(int amount)
    {
        Coin += amount;
        PlayerPrefs.SetFloat("Coin", Coin);
        uiManager.UpdateCoinUI(Coin);
    }

    public void AddGem(int amount)
    {
        Gem += amount;
        PlayerPrefs.SetInt("Gem", Gem);
        uiManager.UpdateGemUI(Gem);
    }

    public void AddCoffeeBean(int amount)
    {
        CoffeeBeans += amount;
        PlayerPrefs.SetInt("CoffeeBean", CoffeeBeans);
       uiManager.UpdateCoffeeBeanUI(CoffeeBeans);
    }


    private int CalculateMaxExp(int currentLevel)
    {
        return 100 + (currentLevel - 1) * 20;
    }
  
}
