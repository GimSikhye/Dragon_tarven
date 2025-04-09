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

        foreach (var cond in quest.conditions) // ���� �ϳ��� UI�� ���� �� ǥ��
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            string state = cond.currentAmount >= cond.requiredAmount ? "(�Ϸ�)" : "";
            text.text = $"({cond.type}) {cond.targetItemId} {cond.currentAmount}/{cond.requiredAmount} {state}"; // ��: (SellItem) americano 2/3, (PlaceFurniture) table_01 1/1 (�Ϸ�)
        }

        questPanel.SetActive(true);
    }

    public void ShowQuestComplete(QuestData quest)
    {
        currentQuest = quest;
        completePopup.SetActive(true);
    }
}
