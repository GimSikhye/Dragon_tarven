using DalbitCafe.Operations;
using UnityEngine;

public class PlayerStatsManager : MonoSingleton<PlayerStatsManager>
{
    public PlayerStats statsSO;
    public int CoffeeBeans { get; private set; }
    public int Coin { get; private set; }
    public int Gem { get; private set; }
    public int Exp { get; private set; }
    public int Level { get; private set; }
    public int MaxExp { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        //InitializeStat("Coin", statsSO.coin);
        InitializeStat("Coin", 100); // �ϴ� �׽�Ʈ�� �׽� �� �ٲٱ�
        InitializeStat("Gem", 10);
        InitializeStat("CoffeeBean", 1000);
        InitializeStat("Exp", 100);
        InitializeStat("Level", 1);
    }


    void InitializeStat(string key, int defaultValue)
    {
        //if (!PlayerPrefs.HasKey(key))
        //{
            PlayerPrefs.SetInt(key, defaultValue);
        //}
    }

    public void Load()
    {
        CoffeeBeans = PlayerPrefs.GetInt("CoffeeBean");
        Coin = PlayerPrefs.GetInt("Coin");
        Gem = PlayerPrefs.GetInt("Gem");
        Exp = PlayerPrefs.GetInt("Exp");
        Level = PlayerPrefs.GetInt("Level");
        MaxExp = CalculateMaxExp(Level);

        UIManager.Instance.UpdateCoinUI(Coin);
        UIManager.Instance.UpdateGemUI(Gem);
        UIManager.Instance.UpdateCoffeeBeanUI(CoffeeBeans);
        UIManager.Instance.UpdateExpUI(Exp, MaxExp, Level);

    }

    public void AddCoin(int amount)
    {
        Coin += amount;
        PlayerPrefs.SetInt("Coin", Coin);
        UIManager.Instance.UpdateCoinUI(Coin);
    }

    public void AddGem(int amount)
    {
        Gem += amount;
        PlayerPrefs.SetInt("Gem", Gem);
        UIManager.Instance.UpdateGemUI(Gem);
    }

    public void AddCoffeeBean(int amount)
    {
        CoffeeBeans += amount;
        PlayerPrefs.SetInt("CoffeeBean", CoffeeBeans);
        UIManager.Instance.UpdateCoffeeBeanUI(CoffeeBeans);
    }

    public void AddExp(int amount)
    {
        Exp += amount;

        while (Exp >= MaxExp)
        {
            Exp -= MaxExp;
            Level++;
            MaxExp = CalculateMaxExp(Level); // MaxExp ���( ���߿� )
        }

        PlayerPrefs.SetInt("Exp", Exp);
        PlayerPrefs.SetInt("Level", Level);

        UIManager.Instance.UpdateExpUI(Exp, MaxExp, Level);
    }

    private int CalculateMaxExp(int currentLevel)
    {
        return 100 + (currentLevel - 1) * 20;
    }

    // ����: PlayerStatsManager �ȿ� ������ �Լ� ������
    public void LevelUp()
    {
        Level++; // ���� ����

        // ��Ÿ ������ ó��...

      CoffeeMachineManager.Instance.UpdateMachineActivation(Level);
    }

}
