using UnityEngine;

// Enum 정의
public enum EventNum
{
    Prologue = 0,
    FirstChapter,
    SecondChapter,
    ThirdChapter,
    FourthChapter,
    Ending
}

[CreateAssetMenu(menuName = "SO/DialogueData")]

public class DialogueData : ScriptableObject
{
    //개인의 대사를 하나하나 담음.
    public Sprite CharacterSprite;
    public string CharacterName;
    public EventNum eventNum; //이벤트 번호
    public Sprite ChangeBG;
    public string[] Lines; // 대사들
}
