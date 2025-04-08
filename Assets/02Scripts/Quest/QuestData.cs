using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "SO/QuestData")]
public class QuestData : ScriptableObject
{
    public Sprite icon;
    public string questTitle;
    public string description;

    public QuestType questType; // 여기도 오타 주의!
    public QuestCondition[] conditions; // <-- 조건들을 배열로 넣어줌

    public int rewardGold;
    public int rewardExp;

    public bool isCompleted;

    public QuestData nextQuest; // 연계 퀘스트
}
