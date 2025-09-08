using System;
using DalbitCafe.Operations;
using UnityEngine;

public class PlayerStatsManager : MonoSingleton<PlayerStatsManager>
{
    public PlayerStats statsSO;
    private UIManager uiManager;
    public int CoffeeBeans { get; private set; }
    public int Coin { get; private set; }
    public int Gem { get; private set; }
    public static event Action<int> OnCoinChanged;

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

    private void OnEnable()
    {
        
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
        if (uiManager == null)
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        }

        Coin += amount;
        PlayerPrefs.SetInt("Coin", Coin);
        PlayerPrefs.Save();
        OnCoinChanged?.Invoke(Coin);
    }

    public void AddGem(int amount)
    {
        if (uiManager == null) uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        Gem += amount;
        PlayerPrefs.SetInt("Gem", Gem);
        PlayerPrefs.Save();

        uiManager.UpdateGemUI(Gem);
    }

    public void AddCoffeeBean(int amount)
    {
        if (uiManager == null) uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        CoffeeBeans += amount;
        PlayerPrefs.SetInt("CoffeeBean", CoffeeBeans);
        PlayerPrefs.Save();

        uiManager.UpdateCoffeeBeanUI(CoffeeBeans);
    }



  
}
