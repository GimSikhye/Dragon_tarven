using UnityEngine;

// 보상 지급 처리 담당
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
        PlayerStats.Instance.gold += quest.rewardGold;
        PlayerStats.Instnace.exp += quest.rewardExp;
        Debug.Log($"퀘스트 완료! 보상 지금 {quest.rewardGold}G / {quest.rewardExp}EXP");
    }
}
