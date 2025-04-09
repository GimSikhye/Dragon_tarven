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
            // ���� ĳ���Ͱ� ��� ���� �� ���� ����
            SetCharacter(centerCharacterImage, centerGroup, line.speaker.characterSprite, 1f);
            SetCharacter(leftCharacterImage, leftGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, null, 0f);
        }
        else
        {
            // ���� ĳ���ʹ� ����, �� ĳ���ʹ� ������
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

        // ĳ���� �̹����� ��� ��Ȱ��ȭ
        SetCharacter(leftCharacterImage, leftGroup, null, 0f);
        SetCharacter(centerCharacterImage, centerGroup, null, 0f);
        SetCharacter(rightCharacterImage, rightGroup, null, 0f);

        // �ʿ��� ���� ���� or UI ��Ȱ��ȭ
        gameObject.SetActive(false); // Ȥ�� ���̾�α� �гθ� ��Ȱ��ȭ
    }

}
