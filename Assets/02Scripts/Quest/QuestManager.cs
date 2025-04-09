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
        foreach (var quest in activeQuests.ToArray()) // ���� ���� ���� ����Ʈ ����Ʈ activeQuests�� ������ �迭�� ��ȸ // ToArray�� ���� ������ ���߿� ����Ʈ�� ���ŵǾ �ݺ��� ������ �ʰ� �ϱ� ���ؼ�.
        {
            bool allComplete = true; // ����Ʈ�� ��� ������ �����ߴ��� ����

            foreach (var condition in quest.conditions)
            {
                if (condition.type == type && condition.targetItemId == itemId && condition.currentAmount < condition.requiredAmount)
                {
                    condition.currentAmount += amount;
                    if (condition.currentAmount > condition.requiredAmount)
                        condition.currentAmount = condition.requiredAmount;
                }

                if (condition.currentAmount < condition.requiredAmount) // ���� �� ä�� ������ �ִٸ� ����Ʈ�� �̿Ϸ�� ����
                    allComplete = false;
            }

            if (allComplete && !quest.isCompleted) // ��� ���� �Ϸ�Ǿ���, ���� �Ϸ� ǥ�ð� �� �� ����Ʈ��� �Ϸ� ó��
            {
                quest.isCompleted = true;
                QuestUI.Instance.ShowQuestComplete(quest);
            }
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        RewardManager.Instance.GiveReward(quest.rewardGold, quest.rewardExp); // ������ ����

        RemoveQuest(quest); // ����Ʈ ����
    }
}
