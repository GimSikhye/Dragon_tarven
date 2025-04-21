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
    public CharacterInfo speaker; // CharacterInfo SO
    public CharacterExpression expression = CharacterExpression.Default;
    public bool isNarration; // �����̼� ���� (true�̸� ĳ���� �̸� �� �̹��� ����)

    public DialogueEvent[] dialogueTexts; // ��� ����(���� ����)
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject // ���̾�α� ��ü ����(��� ���� ����) = Ʋ
{
    public DialogueLine[] lines;
}
