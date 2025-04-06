using DalbitCafe.Core;
using UnityEngine;

// ���� ���� ó�� ���
public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GiveReward(QuestData quest)
    {
        GameManager.Instance.playerStats.AddCoin(quest.rewardGold);
        GameManager.Instance.playerStats.AddExp(quest.rewardExp);
        Debug.Log($"����Ʈ �Ϸ�! ���� ���� {quest.rewardGold}G / {quest.rewardExp}EXP");
    }
}
