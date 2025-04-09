using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public CharacterInfo speaker;
    [TextArea] public string[] dialogueTexts;
    public CharacterExpression expression = CharacterExpression.Default;
}


[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}
