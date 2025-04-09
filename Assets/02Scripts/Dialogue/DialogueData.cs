using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public CharacterInfo speaker;
    public string[] dialogueTexts;
    public CharacterExpression expression;
    public bool isNarration; // ¡ç Ãß°¡µÊ
}


[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}
