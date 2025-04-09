using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image leftCharacterImage;
    public Image centerCharacterImage;
    public Image rightCharacterImage;

    public CanvasGroup leftGroup;
    public CanvasGroup centerGroup;
    public CanvasGroup rightGroup;

    [Header("Typing")]
    public float typingSpeed = 0.05f;

    [Header("Dialogue Data")]
    public DialogueData dialogueData;

    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private CharacterInfo currentSpeaker;



    void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        currentLine = 0;
        ShowLine();
    }

    public void OnClickNext()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueData.lines[currentLine].dialogueText;
            typingCoroutine = null;
            return;
        }

        currentLine++;
        if (currentLine < dialogueData.lines.Length)
        {
            ShowLine();
        }
        else
        {
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

        nameText.text = line.speaker.characterName;
        dialogueText.text = "";

        UpdateCharacters(line);
        typingCoroutine = StartCoroutine(TypeText(line.dialogueText));
        currentSpeaker = line.speaker;
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
        if (currentSpeaker == null || line.speaker == currentSpeaker)
        {
            // 같은 캐릭터가 계속 말함 → 센터 유지
            SetCharacter(centerCharacterImage, centerGroup, line.speaker.characterSprite, 1f);
            SetCharacter(leftCharacterImage, leftGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, null, 0f);
        }
        else
        {
            // 기존 캐릭터는 왼쪽, 새 캐릭터는 오른쪽
            SetCharacter(leftCharacterImage, leftGroup, currentSpeaker.characterSprite, 0.5f);
            SetCharacter(centerCharacterImage, centerGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, line.speaker.characterSprite, 1f);
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

        // 캐릭터 이미지들 모두 비활성화
        SetCharacter(leftCharacterImage, leftGroup, null, 0f);
        SetCharacter(centerCharacterImage, centerGroup, null, 0f);
        SetCharacter(rightCharacterImage, rightGroup, null, 0f);

        // 필요한 종료 연출 or UI 비활성화
        gameObject.SetActive(false); // 혹은 다이얼로그 패널만 비활성화
    }

}
