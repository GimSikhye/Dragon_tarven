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
        foreach (var quest in activeQuests.ToArray()) // 현재 진행 중인 퀘스트 리스트 activeQuests를 복사한 배열로 순회 // ToArray를 쓰는 이유는 도중에 퀘스트가 제거되어도 반복이 꼬이지 않게 하기 위해서.
        {
            bool allComplete = true; // 퀘스트가 모든 조건을 충족했는지 여부

            foreach (var condition in quest.conditions)
            {
                if (condition.type == type && condition.targetItemId == itemId && condition.currentAmount < condition.requiredAmount)
                {
                    condition.currentAmount += amount;
                    if (condition.currentAmount > condition.requiredAmount)
                        condition.currentAmount = condition.requiredAmount;
                }

                if (condition.currentAmount < condition.requiredAmount) // 아직 덜 채운 조건이 있다면 퀘스트는 미완료로 간주
                    allComplete = false;
            }

            if (allComplete && !quest.isCompleted) // 모든 조건 완료되었고, 아직 완료 표시가 안 된 퀘스트라면 완료 처리
            {
                quest.isCompleted = true;
                QuestUI.Instance.ShowQuestComplete(quest);
            }
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        RewardManager.Instance.GiveReward(quest.rewardGold, quest.rewardExp); // 보상을 지급

        RemoveQuest(quest); // 퀘스트 제거
    }
}
