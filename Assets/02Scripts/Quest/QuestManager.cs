using System.Collections.Generic;
using UnityEngine;

// �Ǹ�/��ġ �� ���� üũ�� �̺�Ʈ ����
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Transform questListContent; // ����Ʈ UI�� ������ �� �ʿ��� �θ� ��ü
    public GameObject questItemPrefab;
    public List<QuestData> activeQuests = new(); // ���� �������� ����Ʈ ���

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
            if (child.GetComponent<QuestUIItem>().quest == quest) // ����� ���� ����Ʈ����
            {
                Destroy(child.gameObject);
                break; // �� �̻� ã�� �ʿ� �����ϱ� �ݺ��� Ż��
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

            // ���� ���� �ִ� ����Ʈ�� ������ �ڵ� ����
            if (QuestUI.Instance != null && QuestUI.Instance.IsShowingQuest(quest))
            {
                QuestUI.Instance.UpdateQuestInfo();
            }
        }

        // ����Ʈ UI�� ����
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
        RewardManager.Instance.GiveReward(quest.rewardGold, quest.rewardExp); // ������ ����

        RemoveQuest(quest); // ����Ʈ ����
    }
}
