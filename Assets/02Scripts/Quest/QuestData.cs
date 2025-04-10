using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "SO/QuestData")]
public class QuestData : ScriptableObject
{
    public Sprite icon;
    public string questTitle;
    public string description;

    public QuestType questType; 
    public QuestCondition[] conditions; // <-- 조건들을 배열로 넣어줌

    public int rewardGold; //퀘스트 전체를 완료했을 때 주는 보상
    public int rewardExp;

    public bool isCompleted;

    // 스토리 퀘스트 여부 및 연결된 대사
    public bool isStoryQuest;
    public DialogueData storyDialogue;

    public QuestData nextQuest; // 연계 퀘스트
}
