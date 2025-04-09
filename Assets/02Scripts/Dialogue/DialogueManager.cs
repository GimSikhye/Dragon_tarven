using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image leftCharacterImage;
    public Image centerCharacterImage;
    public Image rightCharacterImage;
    public GameObject nameArea; 


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
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueData.lines[currentLine].dialogueTexts[currentTextIndex];
            typingCoroutine = null;
            return;
        }

        currentTextIndex++;
        if (currentTextIndex < dialogueData.lines[currentLine].dialogueTexts.Length)
        {
            ShowLine();
            return;
        }

        currentLine++;
        currentTextIndex = 0;

        if (currentLine < dialogueData.lines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }


    void ShowLine()
    {
        DialogueLine line = dialogueData.lines[currentLine];

        nameText.text = line.isNarration ? "" : line.speaker.characterName;
        dialogueText.text = "";

        UpdateCharacters(line);
        typingCoroutine = StartCoroutine(TypeText(line.dialogueTexts[currentTextIndex]));

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
        Sprite expressionSprite = line.speaker.GetExpressionSprite(line.expression);

        if (line.isNarration)
        {
            SetCharacter(leftCharacterImage, leftGroup, null, 0f);
            SetCharacter(centerCharacterImage, centerGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, null, 0f);
            return;
        }

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
        bool isNewSprite = image.sprite != sprite;

        image.sprite = sprite;
        image.gameObject.SetActive(sprite != null);

        // 부드러운 페이드 인/아웃
        group.DOFade(alpha, 0.3f);

        // 감정 변화 시 팝업 효과
        if (sprite != null && isNewSprite)
        {
            image.transform.localScale = Vector3.one * 0.8f;
            image.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }
    public Image screenOverlay; // 검은 반투명 배경 이미지

    // 사용법 : ApplyEmotionEffect(line.expression);

    void ApplyEmotionEffect(CharacterExpression expression)
    {
        switch (expression)
        {
            case CharacterExpression.Sad:
                screenOverlay.DOFade(0.3f, 0.5f); // 어두워짐
                break;
            default:
                screenOverlay.DOFade(0f, 0.5f); // 원래 밝기
                break;
        }
    }



    void EndDialogue()
    {
        dialogueText.text = "";
        nameText.text = "";

        SetCharacter(leftCharacterImage, leftGroup, null, 0f);
        SetCharacter(centerCharacterImage, centerGroup, null, 0f);
        SetCharacter(rightCharacterImage, rightGroup, null, 0f);

        gameObject.SetActive(false); // or just hide dialogue panel
    }
}
