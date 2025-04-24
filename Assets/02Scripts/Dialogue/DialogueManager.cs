using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DialogueManager : MonoBehaviour
{
    // ��������ó�� ������ ������۾� ������ �ؾ��ϳ�?
    // �̰� ���� �� �ٲ𶧷� �ٲٱ�
    [Header("UI Components")]
    [FormerlySerializedAs("nameText")]
    public TextMeshProUGUI nameText;
    [FormerlySerializedAs("dialogueText")]
    public TextMeshProUGUI dialogueText;
    [FormerlySerializedAs("nameArea")]
    public GameObject nameArea;

    [Header("���ĵ� �Ϸ���Ʈ ��ġ")]
    public Image leftCharacterImage;
    public Image centerCharacterImage;
    public Image rightCharacterImage;

    [Header("���ĵ� �Ϸ���Ʈ ����ε�")]
    public CanvasGroup leftGroup; // ĳ���� ����ε�
    public CanvasGroup centerGroup;
    public CanvasGroup rightGroup;

    [Header("Typing")]
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Dialogue Data")]
    public DialogueData dialogueData; // ��ȭ ������

    private int currentLine = 0; // ū ����(DialogueLine)
    private int currentTextIndex = 0; // ���� ����(dialogueTexts)
    private Coroutine typingCoroutine; // ���� Ÿ���� ������ Ȯ���ϴ� �뵵

    private CharacterInfo currentSpeaker; // ���� ���ϴ� ĳ����
    private CharacterExpression currentSpeakerExpression = CharacterExpression.Default; // �⺻ ����ǥ��

    [SerializeField] private GameObject imageEffectObject; // �뵵?
    [SerializeField] private Image effectImage;
    public AudioSource sfxSource;

    public bool isStoryDialogue = false; // ���丮 ?

    private void Start() // ���� ����Ʈ �������� (���丮 ����Ʈ���)
    {
        string nextDialogueName = PlayerPrefs.GetString("NextDialogue", ""); // ��������. �⺻ ""

        //if (!string.IsNullOrEmpty(nextDialogueName)) //  ""�� �ƴ϶��
        //{
        //    DialogueData data = Resources.Load<DialogueData>("Dialogues/" + nextDialogueName);
        //    if (data != null)
        //    {
        //        LoadDialogue(data); // Load�Լ� ����
        //    }
        //    // �ѹ� �ε��ϰ� ���� ������ �� ��ȭ�� �� ������� �ʵ��� ����
        //    PlayerPrefs.DeleteKey("NextDialogue");
        //}
        // ������ ���߿� ��ġ��

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
        StartDialogue(); // ù ���� �ڵ� ����
    }

    public void OnClickNext()
    {
        if (typingCoroutine != null) // Ÿ���� ���̶��
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

            line = dialogueData.lines[currentLine]; // ���� �ٷ� �Ѿ���� ������ �����͸� ������������ ����
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
            Debug.LogError("��ȭ �ؽ�Ʈ �ε��� ������ �ʰ��߽��ϴ�.");
            EndDialogue();
            return;
        }


        nameArea.SetActive(!line.isNarration); // �����̼��� �ƴ� ��� SetActive(true) 

        nameText.text = line.isNarration ? "" : line.speaker.characterName;
        dialogueText.text = "";

        UpdateCharacters(line); // UpdateCharacters
        typingCoroutine = StartCoroutine(TypeTextRoutine(line.dialogueTexts[currentTextIndex].text));

        if (!line.isNarration)
        {
            currentSpeaker = line.speaker;
            currentSpeakerExpression = line.expression;
        }

        if (line.dialogueTexts[currentTextIndex].sfx != null)
        {
            sfxSource.PlayOneShot(line.dialogueTexts[currentTextIndex].sfx); // �⺻����? �̰͵� ���߿� soundManager���� �������� ����
        }

        if (line.dialogueTexts[currentTextIndex].image != null)
        {
            effectImage.sprite = line.dialogueTexts[currentTextIndex].image;
            imageEffectObject?.SetActive(true);
        }
        else
        {
            imageEffectObject?.SetActive(false);
        }
    }

    IEnumerator TypeTextRoutine(string text)
    {
        dialogueText.text = ""; // ���� ��� ����
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }
        typingCoroutine = null;
    }

    void UpdateCharacters(DialogueLine line) // �������
    {
        if (line.isNarration)
        {
            SetCharacter(leftCharacterImage, leftGroup, null, 0f);
            SetCharacter(centerCharacterImage, centerGroup, null, 0f);
            SetCharacter(rightCharacterImage, rightGroup, null, 0f);
            return;
        }

        Sprite expressionSprite = line.speaker.GetExpressionSprite(line.expression);

        if (currentSpeaker == null || line.speaker == currentSpeaker) // ó�� �����ϴ� ���ų�, ���� ȭ�ڿ� ���ٸ�
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
