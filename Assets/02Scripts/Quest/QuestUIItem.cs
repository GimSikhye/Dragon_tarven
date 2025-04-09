using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIItem : MonoBehaviour
{
    public Image icon; // ����Ʈ�� ǥ���� ������
    public TextMeshProUGUI progressText; // ���� ���� ���� (2/3 ���� �ؽ�Ʈ)
    public QuestData quest; // �� �׸��� ��Ÿ���� ����Ʈ ������


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


    public void OnClick() // �ش� ����Ʈ �׸��� Ŭ������ ��, ����Ʈ �� ������ �����ִ� �г��� �����ִ� ����
    {
        QuestUI.Instance.ShowQuest(quest);
    }
}
