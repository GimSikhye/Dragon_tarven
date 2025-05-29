using UnityEngine;

// 그냥 독립된 public class인데 같은 파일에 같이 정의된 것뿐
// 클래스 바깥에 정의되어 있고 중첩 안 돼 있으면? ⇒ 완전 독립된 클래스임.
[System.Serializable]
public class DialogueEvent // 대사 한줄 안의 이벤트
{
    [TextArea]
    public string text;
    public AudioClip sfx; // 효과음
    public Sprite image;  // 표시할 이미지
}

[System.Serializable]
public class DialogueLine // 대사 묶음
{
    public CharacterInfo speaker; // CharacterInfo SO (말하는 사람)
    public bool isNarration; // 나레이션 여부 (true이면 캐릭터 이름 및 이미지 숨김)
    public bool isInnerFeelings; //속마음(이름은 안숨기고 글자만 보라색

    public DialogueEvent[] dialogueTexts; // 대사 한줄씩
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "SO/DialogueData")]
public class DialogueData : ScriptableObject // 대사 전체 묶음(다이얼로그)
{
    public DialogueLine[] lines;
}
