using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "SO/QuestData")]
public class QuestData : ScriptableObject
{
    public Sprite icon;
    public string questTitle;
    public string description;

    public QuestType questType; 
    public QuestCondition[] conditions; // <-- ���ǵ��� �迭�� �־���

    public int rewardGold; //����Ʈ ��ü�� �Ϸ����� �� �ִ� ����
    public int rewardExp;

    public bool isCompleted;

    public QuestData nextQuest; // ���� ����Ʈ
}
