using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    [Header(("퀘스트 팝업"))]
    public GameObject questPanel;
    public TextMeshProUGUI questTitleText; // 퀘스트 제목
    public TextMeshProUGUI questDescText; // 퀘스트 설명
    public Transform conditionParent; // 퀘스트 조건이 추가될 위치
    public GameObject conditionTextPrefab; // 퀘스트 조건

    [Header(("퀘스트 완료 팝업"))]
    public GameObject completePopup;
    public Button completeButton;
    public TextMeshProUGUI completeQuestNameText; // 퀘스트 이름
    public Transform rewardArea; // 보상이 위치될 영역
    public GameObject rewardBorderPrefab; // 보상 목록아이콘 프리팹

    [Header("보상 스프라이트")]
    public Sprite goldSprite;
    public Sprite expSprite;

    private QuestData _currentQuest; // 현재 퀘스트
    private List<TextMeshProUGUI> _conditionTextList = new List<TextMeshProUGUI>(); //  UI 텍스트 컴포넌트 참조를 저장하는 용도
    public QuestData CurrentQuest => _currentQuest;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowQuestCondition(QuestData quest) // 해당 퀘스트 조건을 새로 보여줄 때
    {
        _currentQuest = quest;

        questTitleText.text = quest.questTitle;
        questDescText.text = quest.description;

        foreach (Transform child in conditionParent) // 이전 퀘스트의 조건 UI를 다 지움(왜 안되징)
            Destroy(child.gameObject);

        _conditionTextList.Clear(); // 조건 텍스트 리스트도 초기화

        foreach (var cond in quest.conditions) // 해당 퀘스트의 조건들을 가져옴
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            _conditionTextList.Add(text); // 왜 tmp를 추가해주지?
        }

        UpdateQuestInfo(); ///// 이것도 봐야함

    }

    public void UpdateQuestInfo() // 퀘스트 정보 업데이트
    {
        if (_currentQuest == null) return;

        for (int i = 0; i < _currentQuest.conditions.Count(); i++)
        {
            var cond = _currentQuest.conditions[i];
            var text = _conditionTextList[i];

            string state = cond.currentAmount >= cond.requiredAmount ? "(완료)" : "";

            string displayName = GetDisplayName(cond.targetItemId);
            string conditionText = "";

            switch (cond.type)
            {
                case QuestConditionType.SellItem:
                    conditionText = $"{displayName} {cond.requiredAmount}잔 판매 {cond.currentAmount}/{cond.requiredAmount} {state}";
                    break;
                case QuestConditionType.PlaceFurniture:
                    conditionText = $"{displayName} {cond.requiredAmount}개 배치 {cond.currentAmount}/{cond.requiredAmount} {state}";
                    break;
                case QuestConditionType.UpgradeInterior:
                    conditionText = $"{displayName} 업그레이드 {cond.currentAmount}/{cond.requiredAmount} {state}";
                    break;
            }

            text.text = conditionText;
        }
    }

    public bool IsShowingQuest(QuestData quest)
    {
        return _currentQuest == quest;
    }

    public void ShowQuestComplete(QuestData quest)
    {
        _currentQuest = quest;
        completePopup.SetActive(true);

        // 1. 퀘스트 이름 설정
        completeQuestNameText.text = quest.questTitle;

        // 2. 보상 UI 초기화
        foreach (Transform child in rewardArea)
            Destroy(child.gameObject);

        // 3. 보상 종류별 생성
        if (quest.rewardGold > 0)
            CreateRewardUI(goldSprite, quest.rewardGold);

        if (quest.rewardExp > 0)
            CreateRewardUI(expSprite, quest.rewardExp);
        completeButton.onClick.RemoveAllListeners(); // 중복 방지
        completeButton.onClick.AddListener(() =>
        {
            completePopup.SetActive(false);
            GameManager.Instance.QuestManager.CompleteQuest(_currentQuest);
            Debug.Log("완료버튼 누름");
        });

    }

    private void CreateRewardUI(Sprite icon, int amount)
    {
        var go = Instantiate(rewardBorderPrefab, rewardArea);
        var iconImg = go.transform.Find("UI_RewardIcon").GetComponent<Image>();
        var amountText = go.transform.Find("UI_RewardAmountText").GetComponent<TextMeshProUGUI>();

        iconImg.sprite = icon;
        amountText.text = amount.ToString();
    }

    private string GetDisplayName(string id)
    {
        Dictionary<string, string> nameMap = new Dictionary<string, string>()
        {
            { "americano", "아메리카노" },
            { "latte", "라떼" },
            { "espresso", "에스프레소" },
            { "chair_01", "의자1" },
            { "table_01", "테이블1" },
            { "counter_01", "카운터1" },
        };

        return nameMap.TryGetValue(id, out var name) ? name : id;
    }
}
