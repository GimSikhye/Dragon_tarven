using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DialogueManager : MonoSingleton<DialogueManager>   
{
    // 역전재판처럼 독백은 보라색글씨 등으로 해야하나?
    // 이거 참조 씬 바뀔때로 바꾸기
    [Header("UI Components")]
    [FormerlySerializedAs("nameText")]
    public TextMeshProUGUI nameText;
    [FormerlySerializedAs("dialogueText")]
    public TextMeshProUGUI dialogueText;
    [FormerlySerializedAs("nameArea")]
    public GameObject nameArea;

    [Header("Name Plate")]
    public Image namePlateImage; // 이름 이미지

    [Header("스탠딩 일러스트 위치")]
    public Image centerCharacterImage;

    [Header("스탠딩 일러스트 블라인드")]
    public CanvasGroup centerGroup;

    [Header("Typing")]
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Dialogue Data")]
    public DialogueData dialogueData; // 대화 데이터

    [SerializeField] private CameraShake cameraShake;

    private int currentLine = 0; // 큰 단위(DialogueLine)
    private int currentTextIndex = 0; // 작은 단위(dialogueTexts)
    private Coroutine typingCoroutine; // 현재 타이핑 중인지 확인하는 용도

    private CharacterInfo currentSpeaker; // 현재 말하는 캐릭터

    [SerializeField] private GameObject imageEffectObject; // 용도?
    [SerializeField] private Image effectImage;
    public AudioSource sfxSource;

    public bool isStoryDialogue = false; // 스토리 ?

    private void Start() // 다음 퀘스트 가져오기 (스토리 퀘스트라면)
    {
        string nextDialogueName = PlayerPrefs.GetString("NextDialogue", ""); // 가져오기. 기본 ""

        //if (!string.IsNullOrEmpty(nextDialogueName)) //  ""가 아니라면
        //{
        //    DialogueData data = Resources.Load<DialogueData>("Dialogues/" + nextDialogueName);
        //    if (data != null)
        //    {
        //        LoadDialogue(data); // Load함수 로직
        //    }
        //    // 한번 로드하고 나면 재진입 시 대화가 또 실행되지 않도록 삭제
        //    PlayerPrefs.DeleteKey("NextDialogue");
        //}
        // 오류는 나중에 고치기

    }

    public void StartDialogue()
    {
        currentLine = 0;
        currentTextIndex = 0;
        //ShowLine();

        ShowLineImmediate();
    }

    void ShowLineImmediate()
    {
        DialogueLine line = dialogueData.lines[currentLine];
        var textBlock = line.dialogueTexts[currentTextIndex];

        nameArea.SetActive(!line.isNarration);
        namePlateImage.gameObject.SetActive(!line.isNarration);

        if (line.isNarration)
            dialogueText.color = Color.white;
        else if (line.isInnerFeelings)
            dialogueText.color = new Color32(161, 95, 255, 255);
        else
            dialogueText.color = Color.white;

        nameText.text = line.isNarration ? "" : line.speaker.characterName;
        dialogueText.text = textBlock.text;

        UpdateCharacters(line);

        if (line.doCameraShake && cameraShake != null)
            cameraShake.Shake();

        if (textBlock.sfx != null)
            sfxSource.PlayOneShot(textBlock.sfx);

        if (textBlock.image != null)
        {
            effectImage.sprite = textBlock.image;
            imageEffectObject?.SetActive(true);
        }
        else
        {
            imageEffectObject?.SetActive(false);
        }
    }

    public void LoadDialogue(DialogueData newDialogue)
    {
        dialogueData = newDialogue;
        StartDialogue(); // 첫 문장 자동 실행
    }

    public void OnClickNext()
    {
        if (typingCoroutine != null) // 타이핑 중이라면
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueData.lines[currentLine].dialogueTexts[currentTextIndex].text;
            typingCoroutine = null;
            return;
        }

        currentTextIndex++;
        DialogueLine line = dialogueData.lines[currentLine];

        if (currentTextIndex >= line.dialogueTexts.Length)
        {
            currentLine++;
            currentTextIndex = 0;

            if (currentLine >= dialogueData.lines.Length)
            {
                EndDialogue();
                return;
            }

            line = dialogueData.lines[currentLine]; // 다음 줄로 넘어갔으면 다음줄 데이터를 가져오기위해 저장
        }

        ShowLine();
    }

    public void OnClickSkip()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        EndDialogue();
    }

    void ShowLine()
    {
        DialogueLine line = dialogueData.lines[currentLine];

        if (currentTextIndex >= line.dialogueTexts.Length)
        {
            Debug.LogError("대화 텍스트 인덱스 범위를 초과했습니다.");
            EndDialogue();
            return;
        }

        // 이름표와 텍스트 색상 설정
        if (line.isNarration)
        {
            nameArea.SetActive(false);
            namePlateImage.gameObject.SetActive(false);
            dialogueText.color = Color.white;
        }
        else
        {
            nameArea.SetActive(true);
            namePlateImage.sprite = line.speaker.namePlateSprite;
            namePlateImage.gameObject.SetActive(true);

            if (line.isInnerFeelings)
            {
                dialogueText.color = new Color32(161, 95, 255, 255); // 보라색
            }
            else
            {
                dialogueText.color = Color.white;
            }
        }

        nameText.text = line.isNarration ? "" : line.speaker.characterName;
        dialogueText.text = "";

        UpdateCharacters(line); // UpdateCharacters

        if (line.doCameraShake && cameraShake != null)
        {
            cameraShake.Shake(); //흔들기
        }

        typingCoroutine = StartCoroutine(TypeTextRoutine(line.dialogueTexts[currentTextIndex].text));

        if (!line.isNarration)
        {
            currentSpeaker = line.speaker;
        }

        if (line.dialogueTexts[currentTextIndex].sfx != null)
        {
            sfxSource.PlayOneShot(line.dialogueTexts[currentTextIndex].sfx); // 기본음량? 이것도 나중에 soundManager에서 볼륨조절 설정
        }

        if (line.dialogueTexts[currentTextIndex].image != null)
        {
            effectImage.sprite = line.dialogueTexts[currentTextIndex].image;
            effectImage.color = Color.white; // <-- 추가: 알파값 보장
            imageEffectObject?.SetActive(true);
        }
        else
        {
            imageEffectObject?.SetActive(false);
        }

    }

    IEnumerator TypeTextRoutine(string text)
    {
        dialogueText.text = ""; // 이전 대사 제거
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }
        typingCoroutine = null;
    }

    void UpdateCharacters(DialogueLine line) // 여기부터
    {
        if (line.isNarration)
        {
            SetCharacter(centerCharacterImage, centerGroup, null, 0f);
            return;
        }

        Sprite characterSprite = line.speaker.characterSprite;

        SetCharacter(centerCharacterImage, centerGroup, characterSprite, 1f);
    }

    void SetCharacter(Image image, CanvasGroup group, Sprite sprite, float alpha)
    {
        image.sprite = sprite;
        image.gameObject.SetActive(sprite != null);
        group.alpha = alpha;
    }

    void EndDialogue()
    {
        dialogueText.text = "";
        nameText.text = "";

        SetCharacter(centerCharacterImage, centerGroup, null, 0f);

        SceneManager.LoadScene("GameScene");

        //imageEffectObject?.SetActive(false);
    }
}
