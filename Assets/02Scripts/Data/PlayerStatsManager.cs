using System;
using DalbitCafe.Operations;
using UnityEngine;

public class PlayerStatsManager : MonoSingleton<PlayerStatsManager>
{
    //public PlayerStats statsSO;
    public int CoffeeBeans { get; private set; }
    public int Coin { get; private set; }
    public int Gem { get; private set; }
    public static event Action<int> OnCoinChanged;
    public static event Action<int> OnGemChanged;
    public static event Action<int> OnCoffeeBeanChanged;
    protected override void Awake()
    {
        base.Awake();
        //InitializeStat("Coin", statsSO.coin);
        //InitializeStat("Coin", 100); // 일단 테스트로 항시 값 바꾸기
        //InitializeStat("Gem", 10);
        //InitializeStat("CoffeeBean", 1000);

        LoadStat();
    }


    public void InitializeStat()
    {
        CoffeeBeans = 300;
        Coin = 300;
        Gem = 10;
    }
    //public void InitializeStat(string key, int defaultValue)
    //{
    //    //if (!PlayerPrefs.HasKey(key))
    //    //{
    //    //    PlayerPrefs.SetInt(key, defaultValue);
    //    //}
    //}

    private void OnEnable()
    {
        
    }
    public void LoadStat()
    {

        CoffeeBeans = PlayerPrefs.GetInt("CoffeeBean");
        Coin = PlayerPrefs.GetInt("Coin");
        Gem = PlayerPrefs.GetInt("Gem");


        OnCoinChanged?.Invoke(Coin);
        OnGemChanged?.Invoke(Gem);
        OnCoffeeBeanChanged?.Invoke(CoffeeBeans);

    }

    public void AddCoin(int amount)
    {
        Coin += amount;
        PlayerPrefs.SetInt("Coin", Coin);
        PlayerPrefs.Save();
        OnCoinChanged?.Invoke(Coin);
    }

    public void AddGem(int amount)
    {

        Gem += amount;
        PlayerPrefs.SetInt("Gem", Gem);
        PlayerPrefs.Save();

        OnGemChanged?.Invoke(Gem);
    }

    public void AddCoffeeBean(int amount)
    {
        CoffeeBeans += amount;
        PlayerPrefs.SetInt("CoffeeBean", CoffeeBeans);
        PlayerPrefs.Save();

        OnGemChanged?.Invoke(CoffeeBeans);
    }



  
}
