using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    [Header(("����Ʈ �˾�"))]
    public GameObject questPanel;
    public TextMeshProUGUI questTitleText; // ����Ʈ ����
    public TextMeshProUGUI questDescText; // ����Ʈ ����
    public Transform conditionParent; // ����Ʈ ������ �߰��� ��ġ
    public GameObject conditionTextPrefab; // ����Ʈ ����

    [Header(("����Ʈ �Ϸ� �˾�"))]
    public GameObject completePopup;
    public Button completeButton;
    public TextMeshProUGUI completeQuestNameText; // ����Ʈ �̸�
    public Transform rewardArea; // ������ ��ġ�� ����
    public GameObject rewardBorderPrefab; // ���� ��Ͼ����� ������

    [Header("���� ��������Ʈ")]
    public Sprite goldSprite;
    public Sprite expSprite;

    private QuestData _currentQuest; // ���� ����Ʈ
    private List<TextMeshProUGUI> _conditionTextList = new List<TextMeshProUGUI>(); //  UI �ؽ�Ʈ ������Ʈ ������ �����ϴ� �뵵
    public QuestData CurrentQuest => _currentQuest;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowQuestCondition(QuestData quest) // �ش� ����Ʈ ������ ���� ������ ��
    {
        _currentQuest = quest;

        questTitleText.text = quest.questTitle;
        questDescText.text = quest.description;

        foreach (Transform child in conditionParent) // ���� ����Ʈ�� ���� UI�� �� ����
            Destroy(child.gameObject);

        _conditionTextList.Clear(); // ���� �ؽ�Ʈ ����Ʈ�� �ʱ�ȭ

        foreach (var cond in quest.conditions) // �ش� ����Ʈ�� ���ǵ��� ������
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            _conditionTextList.Add(text);
        }

        UpdateQuestInfo(); ///// �̰͵� ������

    }

    public void UpdateQuestInfo() // ����Ʈ ���� ������Ʈ /// _conditionTextList
    {
        if (_currentQuest == null) return;

        for (int i = 0; i < _currentQuest.conditions.Count(); i++)
        {
            QuestCondition cond = _currentQuest.conditions[i];
            TextMeshProUGUI text = _conditionTextList[i]; ////

            string state = cond.currentAmount >= cond.requiredAmount ? "(�Ϸ�)" : ""; // ���緮�� ����Ʈ �ʿ䷮ �����Ѵٸ� (����Ʈ ���� ���¸� ��Ÿ��)

            string displayName = GetDisplayName(cond.targetItemId); // ������Id�� ���ؼ� ������ �̸� ������
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

    public bool IsShowingQuest(QuestData quest) // ���� �������� �ִ� ����Ʈ�� ���� ����Ʈ�� ������
    {
        return _currentQuest == quest;
    }

    public void ShowQuestCompletePopup(QuestData quest) // ����Ʈ �Ϸ�â
    {
        _currentQuest = quest;
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
        completeButton.onClick.RemoveAllListeners(); // �ߺ� ����
        completeButton.onClick.AddListener(() =>
        {
            completePopup.SetActive(false);
            GameManager.Instance.QuestManager.CompleteQuest(_currentQuest);
            // �Ϸ� ��������, ���丮 ����Ʈ ���� �ϴ°� ���ֱ�
        });

    }

    private void CreateRewardUI(Sprite icon, int amount)
    {
        GameObject go = Instantiate(rewardBorderPrefab, rewardArea);
        Image iconImg = go.transform.Find("UI_RewardIcon").GetComponent<Image>();
        TextMeshProUGUI amountText = go.transform.Find("UI_RewardAmountText").GetComponent<TextMeshProUGUI>();

        iconImg.sprite = icon;
        amountText.text = amount.ToString();
    }

    private string GetDisplayName(string id) // ������ID�� �������� ������ �̸����� ��ȯ
    {// ȭ�鿡 ������
        Dictionary<string, string> nameMap = new Dictionary<string, string>() // Ű-�� ���� ����
        {
            { "americano", "�Ƹ޸�ī��" },
            { "latte", "��" },
            { "espresso", "����������" },
            { "chair_01", "����1" },
            { "table_01", "���̺�1" },
            { "counter_01", "ī����1" },
        };

        return nameMap.TryGetValue(id, out var name) ? name : id; // ��ųʸ����� id Ű�� ã�� �õ��� �Ѵ�. // ã���� true ��ȯ + �׿� �ش��ϴ� ���� name ������ �����. 
    }
}
