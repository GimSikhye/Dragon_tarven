using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI progressText;
    public QuestData quest;

    public void Setup(QuestData questData)
    {
        quest = questData;
        icon.sprite = quest.icon;
        UpdateProgress();
    }

    public void UpdateProgress()
    {
        int total = 0;
        int current = 0;

        foreach (var c in quest.conditions)
        {
            total += c.requiredAmount;
            current += c.currentAmount;
        }

        progressText.text = $"{current}/{total}";
    }

    public void OnClick()
    {
        QusetUI.Instance.ShowQuest(quest);
    }
}
