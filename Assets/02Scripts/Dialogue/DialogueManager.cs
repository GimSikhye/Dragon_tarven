using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// 기능 추가+ 오토모드 / 로그 기능
public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject nameArea; // 이름 출력 영역
    public Image leftCharacterImage;
    public Image centerCharacterImage;
    public Image rightCharacterImage;
    public Image eventImage; // 이벤트 이미지 띄우기 용
    public AudioSource audioSource; // 효과음 재생용

    public CanvasGroup leftGroup;
    public CanvasGroup centerGroup;
    public CanvasGroup rightGroup;

    [Header("Typing")]
    public float typingSpeed = 0.05f;

    [Header("Dialogue Data")]
    public DialogueData dialogueData;

    private int currentLine = 0;
    private int currentTextIndex = 0;
    private Coroutine typingCoroutine;

    private CharacterInfo currentSpeaker;
    private CharacterExpression currentSpeakerExpression = CharacterExpression.Default;

    void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        currentLine = 0;
        currentTextIndex = 0;
        ShowLine();
    }

    public void OnClickNext()
    {
        DialogueLine line = dialogueData.lines[currentLine];
        DialogueEvent currentEvent = line.dialogueTexts[currentTextIndex];

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentEvent.text; // 문자열로 출력
            typingCoroutine = null;
            return;
        }

        currentTextIndex++;

        if (currentTextIndex < line.dialogueTexts.Length)
        {
            ShowLine(); // 다음 문장
        }
        else
        {
            currentLine++;
            currentTextIndex = 0;

            if (currentLine < dialogueData.lines.Length)
                ShowLine();
            else
                EndDialogue();
        }
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
        DialogueEvent currentEvent = line.dialogueTexts[currentTextIndex];

        nameArea.SetActive(!line.isNarration);
        nameText.text = line.isNarration ? "" : line.speaker.characterName;
        dialogueText.text = "";

        // 이미지 처리
        if (eventImage != null)
        {
            eventImage.sprite = currentEvent.image;
            eventImage.gameObject.SetActive(currentEvent.image != null);
        }

        // 효과음 처리
        if (audioSource != null)
        {
            if (currentEvent.sfx != null)
            {
                audioSource.PlayOneShot(currentEvent.sfx);
            }
        }

        UpdateCharacters(line);
        typingCoroutine = StartCoroutine(TypeText(currentEvent.text));

        if (!line.isNarration)
        {
            currentSpeaker = line.speaker;
            currentSpeakerExpression = line.expression;
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

        gameObject.SetActive(false);
    }
}
