using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public PlayerStats statsSO;

    void Awake()
    {
        InitializeStat("Gold", statsSO.coin);
        InitializeStat("Gem", statsSO.gem);
        InitializeStat("CoffeeBeans", statsSO.coffeeBean);
        InitializeStat("Exp", statsSO.exp);
    }

    void InitializeStat(string key, int defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, defaultValue);
        }
    }

    public int GetStat(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public void SetStat(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }
}
