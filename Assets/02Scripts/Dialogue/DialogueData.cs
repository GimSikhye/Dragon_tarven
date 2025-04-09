using UnityEngine;
[System.Serializable]
public class DialogueLine // �� ��� ���� ������
{
    public CharacterInfo speaker;
    public CharacterExpression expression = CharacterExpression.Default;
    [TextArea]
    public string[] dialogueTexts; // �� ĳ���Ͱ� ���� ����
    public bool isNarration; // �����̼� ���� (true�̸� ĳ���� �̸� �� �̹��� ����)
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject // ���̾�α� ��ü ����
{
    public DialogueLine[] lines;
}
