using System.Collections.Generic;
using UnityEngine;

// 판매/배치 등 조건 체크용 이벤트 수신
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Transform questListContent;
    public GameObject questItemPrefab;
    public List<QuestData> activeQuests = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddQuest(QuestData quest)
    {
        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            CreateQuestUI(quest);
        }
    }

    public void RemoveQuest(QuestData quest)
    {
        activeQuests.Remove(quest);

        foreach (Transform child in questListContent)
        {
            if (child.GetComponent<QuestUIItem>().quest == quest)
            {
                Destroy(child.gameObject);
                break;
            }
        }

        if (quest.nextQuest != null)
        {
            AddQuest(quest.nextQuest);
        }
    }

    void CreateQuestUI(QuestData quest)
    {
        GameObject go = Instantiate(questItemPrefab, questListContent);
        go.GetComponent<QuestUIItem>().Setup(quest);
    }

    
    public void CheckQuestProgress(string itemId, QuestConditionType type, int amount = 1)
    {
        foreach (var quest in activeQuests.ToArray())
        {
            bool allComplete = true;

            foreach (var condition in quest.conditions)
            {
                if (condition.type == type && condition.targetItemId == itemId && condition.currentAmount < condition.requiredAmount)
                {
                    condition.currentAmount += amount;
                    if (condition.currentAmount > condition.requiredAmount)
                        condition.currentAmount = condition.requiredAmount;
                }

                if (condition.currentAmount < condition.requiredAmount)
                    allComplete = false;
            }

            if (allComplete && !quest.isCompleted)
            {
                quest.isCompleted = true;
                QusetUI.Instance.ShowQuestComplete(quest);
            }
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        RewardManager.Instance.GiveReward(quest.rewardGold, quest.rewardExp);

        RemoveQuest(quest);
    }
}
