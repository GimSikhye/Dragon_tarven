using System.Collections.Generic;
using UnityEngine;
// ����Ʈ ���� ����, ���� üũ �� �Ϸ� ó��
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

                if (quest.requiredAmount <= 0) // �ʿ��� ���� 0�� �Ǿ��ٸ�
                {
                    quest.isCompleted = true;
                    RewardManager.Instance.GiveReward(quest);
                    QusetUI.Instance.ShowQuestComplete(quest); // UI ������Ʈ
                }
            }
        }
    }
}
