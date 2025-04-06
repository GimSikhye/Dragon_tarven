using System.Collections.Generic;
using UnityEngine;
// 퀘스트 진행 관리, 조건 체크 및 완료 처리
public class QuestManager : MonoBehaviour
{
    public List<QuestData> activeQuests;

    public void CheckQuestProgress(string itemId, QusetType type, int amount = 0)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questType == type && quest.targetItemId == itemId && !quest.isCompleted)
            {
                quest.requiredAmount -= amount;

                if (quest.requiredAmount <= 0) // 필요한 양이 0이 되었다면
                {
                    quest.isCompleted = true;
                    RewardManager.Instance.GiveReward(quest);
                    QusetUI.Instance.ShowQuestComplete(quest); // UI 업데이트
                }
            }
        }
    }
}
