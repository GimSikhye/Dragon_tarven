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

        foreach (var cond in quest.conditions) // 조건 하나씩 UI로 생성 및 표시
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            string state = cond.currentAmount >= cond.requiredAmount ? "(완료)" : "";
            text.text = $"({cond.type}) {cond.targetItemId} {cond.currentAmount}/{cond.requiredAmount} {state}"; // 예: (SellItem) americano 2/3, (PlaceFurniture) table_01 1/1 (완료)
        }

        questPanel.SetActive(true);
    }

    public void ShowQuestComplete(QuestData quest)
    {
        currentQuest = quest;
        completePopup.SetActive(true);
    }
}
