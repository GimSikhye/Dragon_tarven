using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIItem : MonoBehaviour
{
    public Image icon; // 퀘스트에 표시할 아이콘
    public TextMeshProUGUI progressText; // 현재 진행 상태 (2/3 같은 텍스트)
    public QuestData quest; // 이 항목이 나타내는 퀘스트 데이터


    public void Setup(QuestData questData)
    {
        quest = questData;
        icon.sprite = quest.icon;
        UpdateProgress();
    }

    public void UpdateProgress()
    {
        int totalConditions = quest.conditions.Count();
        int completedConditions = 0;

        foreach (var c in quest.conditions)
        {
            if (c.currentAmount >= c.requiredAmount)
            {
                completedConditions++;
            }
        }

        progressText.text = $"{completedConditions}/{totalConditions}";
    }


    public void OnClick() // 해당 퀘스트 항목을 클릭했을 때, 퀘스트 상세 정보를 보여주는 패널을 열어주는 역할
    {
        QuestUI.Instance.ShowQuest(quest);
    }
}
