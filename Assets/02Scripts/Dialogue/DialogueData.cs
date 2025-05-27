using UnityEngine;

// �׳� ������ public class�ε� ���� ���Ͽ� ���� ���ǵ� �ͻ�
// Ŭ���� �ٱ��� ���ǵǾ� �ְ� ��ø �� �� ������? �� ���� ������ Ŭ������.
[System.Serializable]
public class DialogueEvent // ��� ���� ���� �̺�Ʈ
{
    [TextArea]
    public string text;
    public AudioClip sfx; // ȿ����
    public Sprite image;  // ǥ���� �̹���
}

[System.Serializable]
public class DialogueLine // ��� ����
{
    public CharacterInfo speaker; // CharacterInfo SO (���ϴ� ���)
    public bool isNarration; // �����̼� ���� (true�̸� ĳ���� �̸� �� �̹��� ����)
    public bool isInnerFeelings; //�Ӹ���(�̸��� �ȼ���� ���ڸ� �����

    public DialogueEvent[] dialogueTexts; // ��� ���پ�
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject // ��� ��ü ����(���̾�α�)
{
    public DialogueLine[] lines;
}
