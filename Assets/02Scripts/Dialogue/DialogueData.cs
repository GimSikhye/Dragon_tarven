using UnityEngine;

[System.Serializable]
public class DialogueEvent
{
    [TextArea]
    public string text;
    public AudioClip sfx; // 효과음
    public Sprite image;  // 표시할 이미지
}

[System.Serializable]
public class DialogueLine // 각 대사 라인 데이터
{
    public CharacterInfo speaker; // CharacterInfo SO
    public CharacterExpression expression = CharacterExpression.Default;
    public bool isNarration; // 나레이션 여부 (true이면 캐릭터 이름 및 이미지 숨김)

    public DialogueEvent[] dialogueTexts; // 대사 묶음(여러 문장)
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject // 다이얼로그 전체 묶음(대사 묶음 모음) = 틀
{
    public DialogueLine[] lines;
}
