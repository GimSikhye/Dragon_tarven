using System.Collections.Generic;
using UnityEngine;

// 판매/배치 등 조건 체크용 이벤트 수신
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Transform questListContent; // 퀘스트 UI를 생성할 때 필요한 부모 객체
    public GameObject questItemPrefab;
    public List<QuestData> activeQuests = new(); // 현재 진행중인 퀘스트 목록

    public QuestData quest1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AddQuest(quest1);
        QuestManager.Instance.ResetQuestProgress(quest1);
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
            if (child.GetComponent<QuestUIItem>().quest == quest) // 지우고 싶은 퀘스트인지
            {
                Destroy(child.gameObject);
                break; // 더 이상 찾을 필요 없으니까 반복문 탈출
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
                QuestUI.Instance.ShowQuestComplete(quest);
            }

            // 현재 보고 있는 퀘스트와 같으면 자동 갱신
            if (QuestUI.Instance != null && QuestUI.Instance.IsShowingQuest(quest))
            {
                QuestUI.Instance.UpdateQuestInfo();
            }
        }

        // 리스트 UI도 갱신
        foreach (Transform child in questListContent)
        {
            QuestUIItem uiItem = child.GetComponent<QuestUIItem>();
            if (uiItem != null)
            {
                uiItem.UpdateProgress();
            }
        }
    }

    public void ResetQuestProgress(QuestData quest)
    {
        quest.isCompleted = false;

        foreach (var cond in quest.conditions)
        {
            cond.currentAmount = 0;
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        RewardManager.Instance.GiveReward(quest.rewardGold, quest.rewardExp); // 보상을 지급

        RemoveQuest(quest); // 퀘스트 제거
    }
}
