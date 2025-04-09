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

    public void UpdateProgress() // ��ü ������ ��� ���ļ� ���൵�� �����.
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

    public void OnClick() // �ش� ����Ʈ �׸��� Ŭ������ ��, ����Ʈ �� ������ �����ִ� �г��� �����ִ� ����
    {
        QuestUI.Instance.ShowQuest(quest);
    }
}
