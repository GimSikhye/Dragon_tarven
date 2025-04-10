using UnityEngine;
[System.Serializable]
public class DialogueEvent
{
    [TextArea]
    public string text;
    public AudioClip sfx; // ȿ����
    public Sprite image;  // ǥ���� �̹���
}

[System.Serializable]
public class DialogueLine // �� ��� ���� ������
{
    public CharacterInfo speaker;
    public CharacterExpression expression = CharacterExpression.Default;
    public bool isNarration; // �����̼� ���� (true�̸� ĳ���� �̸� �� �̹��� ����)
    public DialogueEvent[] dialogueTexts; // �� ĳ���Ͱ� ���� ����

}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject // ���̾�α� ��ü ����
{
    public DialogueLine[] lines;
}
