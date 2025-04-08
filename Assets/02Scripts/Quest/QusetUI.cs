using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QusetUI : MonoBehaviour
{
    public static QusetUI Instance;

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

    public void ShowQuest(QuestData quest)
    {
        currentQuest = quest;

        questTitleText.text = quest.questTitle;
        questDescText.text = quest.description;

        foreach (Transform child in conditionParent)
            Destroy(child.gameObject);

        foreach (var cond in quest.conditions)
        {
            var go = Instantiate(conditionTextPrefab, conditionParent);
            var text = go.GetComponent<TextMeshProUGUI>();
            string state = cond.currentAmount >= cond.requiredAmount ? "(¿Ï·á)" : "";
            text.text = $"({cond.type}) {cond.targetItemId} {cond.currentAmount}/{cond.requiredAmount} {state}";
        }

        questPanel.SetActive(true);
    }

    public void ShowQuestComplete(QuestData quest)
    {
        currentQuest = quest;
        completePopup.SetActive(true);
    }
}
