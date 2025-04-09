using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public CharacterInfo speaker;
    public string dialogueText;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}
