using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "SO/QuestData")]
public class QuestData : ScriptableObject
{
    public Sprite icon; // 퀘스트 아이콘
    public string questTitle; // 퀘스트 제목
    public string description; // 퀘스트 설명

    public QuestType questType; 
    public QuestCondition[] conditions; // <-- 조건들을 배열로 넣어줌

    public int rewardGold; //퀘스트 전체를 완료했을 때 주는 골드
    public int rewardExp; // 퀘스트 전체를 완료했을 때 주는 경험치

    public bool isCompleted; // 이 퀘스트를 완료했는지

    // 스토리 퀘스트 여부 및 연결된 대사
    public bool isStoryQuest; // 스토리 퀘스트인지
    public DialogueData storyDialogue; 

    public QuestData nextQuest; // 연계 퀘스트
}
