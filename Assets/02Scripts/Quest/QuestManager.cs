using System.Collections.Generic;
using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.SceneManagement;

// 판매/배치 등 조건 체크용 이벤트 수신
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; } 

    private void Awake()
    {
        Instance = this; // 현재 인스턴스를 설정
    }
    [SerializeField] private Transform questCatalogContent; // 퀘스트 UI를 생성할 때 필요한 부모 객체
    public GameObject questItemPrefab; // QuestSelectButton
    public List<QuestData> onGoingQuest = new(); // 계속 진행중인 퀘스트 목록

    public QuestData quest1; // 임시 퀘스트
    private DialogueManager dialougManager;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += Init;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Init;
    }

    private void Init(Scene scene, LoadSceneMode sceneMode)
    {
        if(scene.name == "GameScene")
        {
            //dialougManager = DialogueManager.Instance;
            //questCatalogContent = UIManager.Instance.panels[(int)Windows.Quest].transform.Find("UI_QuestCatalog/Viewport/QuestCatalogContent");
            questItemPrefab = Resources.Load<GameObject>("Prefabs/UI_QuestSelectButton"); //Resources.Load 성능?

            // 테스트(임시)
            quest1 = Resources.Load<QuestData>("QuestData/QuestData1");
            AddQuest(quest1); 
            ResetQuestProgress(quest1);

        }
    }

    public void AddQuest(QuestData quest) // 퀘스트 추가
    {
        if (!onGoingQuest.Contains(quest)) // 진행중인 퀘스트가 해당 퀘스트를 포함하지 않는다면
        {
            onGoingQuest.Add(quest); // 진행중인 퀘스트에 추가
            CreateQuestSelectButton(quest);
        }
    }

    public void RemoveQuest(QuestData quest) // 퀘스트 제거
    {
        onGoingQuest.Remove(quest); // 진행중인 퀘스트에서 제거

        foreach (Transform child in questCatalogContent)
        {
            if (child.GetComponent<QuestUIItem>().quest == quest) // 지우고 싶은 퀘스트인지
            {
                Destroy(child.gameObject);
                break; // 더 이상 찾을 필요 없으니까 반복문 탈출
            }
        }

        if (quest.nextQuest != null) // 해당 퀘스트의 연계 퀘스트가 있다면
        {
            AddQuest(quest.nextQuest);
        }
    }

    private void CreateQuestSelectButton(QuestData quest) // 퀘스트 선택 버튼 생성
    {
        GameObject questSelectButton = Instantiate(questItemPrefab, questCatalogContent); // 여기서 null 남
        questSelectButton.GetComponent<QuestUIItem>().Setup(quest);
    }

    public void CheckQuestProgress(QuestConditionType type, string itemId, int amount = 1) // 퀘스트 진행도 체크
    {
        foreach (var quest in onGoingQuest.ToArray())
        {
            bool allComplete = true; // 모두 완료했는지

            foreach (var condition in quest.conditions) // 퀘스트의 개별 조건들
            {
                if (condition.type == type && 
                    condition.targetItemId == itemId && condition.currentAmount < condition.requiredAmount) // 퀘스트 타입과 아이템이 일치하고, 필요한 수량보다 현재 현재 수량이 적으면
                {
                    condition.currentAmount += amount; // 현재 수량
                    if (condition.currentAmount > condition.requiredAmount) // 현재 수량이 필요한 수량보다 크다면
                        condition.currentAmount = condition.requiredAmount; // 현재 수량 = 필요한 수량으로 갱신
                }

                if (condition.currentAmount < condition.requiredAmount) // 필요한 수량보다 작다면
                    allComplete = false;
            }

            if (allComplete && !quest.isCompleted) // 모두 필요한 수량이 충족되었고, 퀘스트가 완료되지 않았다면
            {
                quest.isCompleted = true; // 퀘스트 완료 처리
                QuestUI.Instance.ShowQuestCompletePopup(quest); // 퀘스트 완료창 띄우기

                if (quest.isStoryQuest && quest.storyDialogue != null) // 스토리 퀘스트이고 퀘스트의 storyDialogue가 null이 아니라면
                {
                    PlayerPrefs.SetString("NextDialogue", quest.storyDialogue.name); // 다음 스토리에, Dialogue 이름 저장
                }
            }

            // 현재 보고 있는 퀘스트와 같으면 자동 갱신
            if (QuestUI.Instance != null && QuestUI.Instance.IsShowingQuest(quest))
            {
                QuestUI.Instance.UpdateQuestInfo();
            }
        }

        // 퀘스트 선택 버튼도 갱신
        foreach (Transform child in questCatalogContent)
        {
            QuestUIItem questSelectButton = child.GetComponent<QuestUIItem>();
            if (questSelectButton != null)
            {
                questSelectButton.UpdateProgress();
            }
        }
    }

    public void ResetQuestProgress(QuestData quest) // 퀘스트 진행도 초기화
    {
        quest.isCompleted = false; // 완료처리된 퀘스트 다시 미완료로 전환

        foreach (var condition in quest.conditions)
        {
            condition.currentAmount = 0; // 조건의 현재 수량들 초기화
        }
    }

    public void CompleteQuest(QuestData quest) // 퀘스트 완료
    {
        RewardManager.Instance.GiveReward(quest.rewardGold, quest.rewardExp); // 퀘스트 보상(골드, 경험치 지급)
        RemoveQuest(quest); // 해당 퀘스트 제거
    }
}
