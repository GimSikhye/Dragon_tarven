using UnityEngine;

[CreateAssetMenu(menuName = "SO/DialogueData")]

public class DialogueData : ScriptableObject
{
    //개인의 대사를 하나하나 담음.
    public Sprite CharacterSprite;
    public string CharacterName;
    public int EventNum; //이벤트 번호
    public Sprite ChangeBG;
    public string[] Lines;
}
