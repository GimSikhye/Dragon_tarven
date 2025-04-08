using UnityEngine;
using DalbitCafe.Core;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GiveReward(int gold, int exp)
    {
        GameManager.Instance.playerStats.AddCoin(gold);
        GameManager.Instance.playerStats.AddExp(exp);
        Debug.Log($"보상 지급: {gold}G / {exp}EXP");
    }
}
