using System.Collections.Generic;
using System.Linq;
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
    private List<TextMeshProUGUI> conditionTextList = new List<TextMeshProUGUI>();

    public QuestData CurrentQuest => currentQuest;


    private void Awake()
    {
        Instance = this;
        completeButton.onClick.AddListener(() =>
        {
            QuestManager.Instance.CompleteQuest(currentQuest);
            completePopup.SetActive(false);
        });
    }

    public void ShowQuest(QuestData quest)
    {
        currentQuest = quest;

        questTitleText.text = quest.questTitle;
        questDescText.text = quest.description;

        foreach (Transform child in conditionParent)
            Destroy(child.gameObject);

        conditionTextList.Clear();

        foreach (var cond in quest.conditions)
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            conditionTextList.Add(text);
        }

        UpdateQuestInfo();

        questPanel.SetActive(true);
    }

    public void UpdateQuestInfo()
    {
        if (currentQuest == null) return;

        for (int i = 0; i < currentQuest.conditions.Count(); i++)
        {
            var cond = currentQuest.conditions[i];
            var text = conditionTextList[i];

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
        return currentQuest == quest;
    }

    public void ShowQuestComplete(QuestData quest)
    {
        currentQuest = quest;
        completePopup.SetActive(true);
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
