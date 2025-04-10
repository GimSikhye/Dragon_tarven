using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public TextMeshProUGUI completeQuestNameText;
    public Transform rewardArea;
    public GameObject rewardBorderPrefab;
    public Sprite goldSprite;
    public Sprite expSprite;

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
            SceneManager.LoadScene("DialogueScene");
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

            string state = cond.currentAmount >= cond.requiredAmount ? "(�Ϸ�)" : "";

            string displayName = GetDisplayName(cond.targetItemId);
            string conditionText = "";

            switch (cond.type)
            {
                case QuestConditionType.SellItem:
                    conditionText = $"{displayName} {cond.requiredAmount}�� �Ǹ� {cond.currentAmount}/{cond.requiredAmount} {state}";
                    break;
                case QuestConditionType.PlaceFurniture:
                    conditionText = $"{displayName} {cond.requiredAmount}�� ��ġ {cond.currentAmount}/{cond.requiredAmount} {state}";
                    break;
                case QuestConditionType.UpgradeInterior:
                    conditionText = $"{displayName} ���׷��̵� {cond.currentAmount}/{cond.requiredAmount} {state}";
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

        // 1. ����Ʈ �̸� ����
        completeQuestNameText.text = quest.questTitle;

        // 2. ���� UI �ʱ�ȭ
        foreach (Transform child in rewardArea)
            Destroy(child.gameObject);

        // 3. ���� ������ ����
        if (quest.rewardGold > 0)
            CreateRewardUI(goldSprite, quest.rewardGold);

        if (quest.rewardExp > 0)
            CreateRewardUI(expSprite, quest.rewardExp);
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
            { "americano", "�Ƹ޸�ī��" },
            { "latte", "��" },
            { "espresso", "����������" },
            { "chair_01", "����1" },
            { "table_01", "���̺�1" },
            { "counter_01", "ī����1" },
        };

        return nameMap.TryGetValue(id, out var name) ? name : id;
    }
}
