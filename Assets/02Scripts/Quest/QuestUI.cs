using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    public GameObject questPanel;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescText;
    public Transform conditionParent;
    public GameObject conditionTextPrefab;

    public GameObject completePopup;
    public Button completeButton;

    private QuestData currentQuest;

    private void Awake()
    {
        Instance = this;
        completeButton.onClick.AddListener(() =>
        {
            QuestManager.Instance.CompleteQuest(currentQuest);
            completePopup.SetActive(false);
        });
    }

    public void ShowQuest(QuestData quest) // 특정 퀘스트를 선택했을 때, 퀘스트 정보를 UI에 보여주는 역할
    {
        currentQuest = quest; // 지금 표시 중인 퀘스트를 저장

        questTitleText.text = quest.questTitle;
        questDescText.text = quest.description;

        foreach (Transform child in conditionParent) // 이전에 표시돼 있던 조건 UI 항목들을 싹 지움
            Destroy(child.gameObject);

        foreach (var cond in quest.conditions)
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();

            string state = cond.currentAmount >= cond.requiredAmount ? "(완료)" : "";

            // 타입별 한글 문장 구성
            string conditionText = "";

            string displayName = GetDisplayName(cond.targetItemId); // 한글 이름 변환 함수

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


        questPanel.SetActive(true);
    }

    private string GetDisplayName(string id)
    {
        // 커피 & 가구 아이디에 따른 한글 이름 매핑
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


    public void ShowQuestComplete(QuestData quest)
    {
        currentQuest = quest;
        completePopup.SetActive(true);
    }
}
