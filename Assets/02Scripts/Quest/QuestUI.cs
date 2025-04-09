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

    public void ShowQuest(QuestData quest) // Ư�� ����Ʈ�� �������� ��, ����Ʈ ������ UI�� �����ִ� ����
    {
        currentQuest = quest; // ���� ǥ�� ���� ����Ʈ�� ����

        questTitleText.text = quest.questTitle;
        questDescText.text = quest.description;

        foreach (Transform child in conditionParent) // ������ ǥ�õ� �ִ� ���� UI �׸���� �� ����
            Destroy(child.gameObject);

        foreach (var cond in quest.conditions)
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();

            string state = cond.currentAmount >= cond.requiredAmount ? "(�Ϸ�)" : "";

            // Ÿ�Ժ� �ѱ� ���� ����
            string conditionText = "";

            string displayName = GetDisplayName(cond.targetItemId); // �ѱ� �̸� ��ȯ �Լ�

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


        questPanel.SetActive(true);
    }

    private string GetDisplayName(string id)
    {
        // Ŀ�� & ���� ���̵� ���� �ѱ� �̸� ����
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


    public void ShowQuestComplete(QuestData quest)
    {
        currentQuest = quest;
        completePopup.SetActive(true);
    }
}
