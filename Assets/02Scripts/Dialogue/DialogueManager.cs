using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    [FormerlySerializedAs("nameText")]
    public TextMeshProUGUI nameText;
    [FormerlySerializedAs("dialogueText")]
    public TextMeshProUGUI dialogueText;
    [FormerlySerializedAs("nameArea")]
    public GameObject nameArea;

    public Image leftCharacterImage;
    public Image centerCharacterImage;
    public Image rightCharacterImage;

    public CanvasGroup leftGroup;
    public CanvasGroup centerGroup;
    public CanvasGroup rightGroup;

    [Header("Typing")]
    public float typingSpeed = 0.05f;

    [Header("Dialogue Data")]
    public DialogueData dialogueData; // 대화 데이터

    private int currentLine = 0;
    private int currentTextIndex = 0;
    private Coroutine typingCoroutine;

    private CharacterInfo currentSpeaker; // 현재 말하는 캐릭터
    private CharacterExpression currentSpeakerExpression = CharacterExpression.Default; // 감정표현

    public GameObject imageEffectObject;
    public Image effectImage;
    public AudioSource sfxSource;

    public bool isStoryDialogue = false;

    private void Start()
    {
        string nextDialogueName = PlayerPrefs.GetString("NextDialogue", ""); // 초기화

        if (!string.IsNullOrEmpty(nextDialogueName)) // 비었다면
        {
            DialogueData data = Resources.Load<DialogueData>("Dialogues/" + nextDialogueName);
            if (data != null)
            {
                LoadDialogue(data);
            }
            else
            {
                Debug.LogWarning("해당 이름의 DialogueData를 찾을 수 없습니다: " + nextDialogueName);
            }

            // 한번 로드하고 나면 재진입 시 대화가 또 실행되지 않도록 삭제
            PlayerPrefs.DeleteKey("NextDialogue");
        }
    }

    public void StartDialogue()
    {
        currentLine = 0;
        currentTextIndex = 0;
        ShowLine();
    }

    public void LoadDialogue(DialogueData newDialogue)
    {
        dialogueData = newDialogue;
        gameObject.SetActive(true);

        StartDialogue(); // 첫 문장 자동 실행
    }


    public void OnClickNext()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueData.lines[currentLine].dialogueTexts[currentTextIndex].text;
            typingCoroutine = null;
            return;
        }

        currentTextIndex++;

        // 텍스트 인덱스가 범위를 벗어나면 다음 라인으로
        if (currentLine >= dialogueData.lines.Length)
        {
            EndDialogue();
            return;
        }

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

            line = dialogueData.lines[currentLine];
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
        if (dialogueData == null || dialogueData.lines == null || currentLine >= dialogueData.lines.Length)
        {
            Debug.LogError("잘못된 대화 데이터입니다.");
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueData.lines[currentLine];

        if (currentTextIndex >= line.dialogueTexts.Length)
        {
            Debug.LogError("대화 텍스트 인덱스 범위를 초과했습니다.");
            EndDialogue();
            return;
        }

        nameArea.SetActive(!line.isNarration);
        nameText.text = line.isNarration ? "" : line.speaker.characterName;
        dialogueText.text = "";

        UpdateCharacters(line);
        typingCoroutine = StartCoroutine(TypeText(line.dialogueTexts[currentTextIndex].text));

        if (!line.isNarration)
        {
            currentSpeaker = line.speaker;
            currentSpeakerExpression = line.expression;
        }

        if (line.dialogueTexts[currentTextIndex].sfx != null)
        {
            sfxSource.PlayOneShot(line.dialogueTexts[currentTextIndex].sfx);
        }

        if (line.dialogueTexts[currentTextIndex].image != null)
        {
            effectImage.sprite = line.dialogueTexts[currentTextIndex].image;
            //imageEffectObject?.SetActive(true);
        }
        else
        {
            //imageEffectObject?.SetActive(false);
        }
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    void UpdateCharacters(DialogueLine line)
    {
        if (line.isNarration)
        {
            SetCharacter(leftCharacterImage, leftGroup, null, 0f);
            SetCharacter(centerCharacterImage, centerGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, null, 0f);
            return;
        }

        Sprite expressionSprite = line.speaker.GetExpressionSprite(line.expression);

        if (currentSpeaker == null || line.speaker == currentSpeaker)
        {
            SetCharacter(centerCharacterImage, centerGroup, expressionSprite, 1f);
            SetCharacter(leftCharacterImage, leftGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, null, 0f);
        }
        else
        {
            SetCharacter(leftCharacterImage, leftGroup, currentSpeaker.GetExpressionSprite(currentSpeakerExpression), 0.5f);
            SetCharacter(centerCharacterImage, centerGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, expressionSprite, 1f);
        }
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

        SetCharacter(leftCharacterImage, leftGroup, null, 0f);
        SetCharacter(centerCharacterImage, centerGroup, null, 0f);
        SetCharacter(rightCharacterImage, rightGroup, null, 0f);

        SceneManager.LoadScene("GameScene");

        //imageEffectObject?.SetActive(false);
    }
}
