using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public enum CoffeeState { BaseSelect, Pouring, Syrup, WhippedCream}
[System.Serializable]
public class BaseSpriteEntry // entry: �׸�
{
    public string baseName;
    public Sprite sprite;
}
public class CoffeeMakingManager : MonoBehaviour
{
    [SerializeField] private GameObject basePanel;
    [SerializeField] private GameObject pouringPanel;
    [SerializeField] private GameObject syrupPanel;

    private CoffeeState currentState;
    private string selectedBase;
    [SerializeField] private BaseSpriteEntry[] baseSpriteEntries;
    private Dictionary<string, Sprite> baseSprites;

    [SerializeField] private Image pourDrink;
    [SerializeField] private Animator pouringAnimator; // �ִϸ��̼� ��Ʈ�ѿ�
    [SerializeField] private float pourSpeed = 10f;
    [SerializeField] private float pourDecreaseSpeed = 5f;
    [SerializeField] private TextMeshProUGUI amountText;

    private float pouredAmount = 0f; // ���� ������ ��
    private float pourIntensity = 0f; // ���� ������� ���� ��
    private Vector2 simulatedTilt = Vector2.zero;
    [SerializeField] private float simulatedTiltSpeed = 1.5f;
    [SerializeField] private float simulatedTiltMax = 1.2f;


    // �÷� ���� Ƚ�� ����� ��ųʸ�
    private Dictionary<string, int> syrupCounts = new();
    [SerializeField] private HorizontalLayoutGroup syrupListPanel;
    [SerializeField] private GameObject syrupLabelPrefab; // �÷��� �߰��Ǿ����� ��Ÿ�� UI ������ (Text)
    [SerializeField] private float textOffset;
    [SerializeField] Transform mug;
    [SerializeField] private Vector3 mugDefaultPosition; // �߾� ��ġ
    [SerializeField] private Vector3 mugOffset = new Vector3(-50f, 0f, 0f); // ���漭 �������� �̵��� ������
    private string lastUsedSyrup = ""; // ���������� ����� �÷� �̸� ����
    private Coroutine returnCoroutine; // ���� ���� ���� ���� �ڷ�ƾ ����
    private float pumpingCooldown;
    [SerializeField] float pumpingCooltime = 0.5f;

    // Ÿ�̸� ���� ������
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private Image timerFillImage;
    [SerializeField] private float totalTime = 120f; // �� �ð�(��)
    private float currentTime;
    private bool isTimerRunning = false;
    private Coroutine timerCoroutine;

    private void Start()
    {
        baseSprites = new Dictionary<string, Sprite>();
        foreach(var entry in baseSpriteEntries)
        {
            baseSprites[entry.baseName] = entry.sprite;
        }

        InitializeTimer();
        SetState(CoffeeState.BaseSelect);

    }
    private void Update()
    {
        if (currentState == CoffeeState.Pouring)
        {
            HandlePouring();
        }

        if (currentState == CoffeeState.Syrup)
        {
            if (pumpingCooldown > 0)
                pumpingCooldown -= Time.deltaTime;
        }
    }
    #region Timer Methods
    private void InitializeTimer()
    {
        currentTime = totalTime;
        UpdateTimerUI();
        StartTimer();

        if(timerFillImage != null)
        {
            timerFillImage.type = Image.Type.Filled;
            timerFillImage.fillMethod = Image.FillMethod.Horizontal;
            timerFillImage.fillOrigin = 0; // ���ʿ��� ����(Left)
            timerFillImage.fillAmount = 1f; // ������ ���� ���� ��
        }

    }

    public void StartTimer()
    {
        if(!isTimerRunning)
        {
            isTimerRunning = true;
            timerCoroutine = StartCoroutine(TimerCountdown());
        }
    }

    public void StopTimer()
    {
        if(isTimerRunning)
        {
            isTimerRunning = false;
            if(timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }
    }

    public void ResetTimer()
    {
        StopTimer();
        currentTime = totalTime;
        UpdateTimerUI();
    }

    private IEnumerator TimerCountdown()
    {
        while(currentTime > 0 && isTimerRunning)
        {
            yield return new WaitForSeconds(1f); // 1�� ���

            currentTime -= 1f;
            currentTime = Mathf.Max(0f, currentTime); // 0 ���Ϸ� �������� �ʵ���

            UpdateTimerUI();
        }

        // Ÿ�̸Ӱ� ������ ��
        if(currentTime <= 0)
        {
            OnTimerEnd();
        }

        isTimerRunning = false;
        timerCoroutine = null;
    }

    private void UpdateTimerUI()
    {
        // �ؽ�Ʈ ������Ʈ (��:�� ����)
        if(timeRemainingText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timeRemainingText.text = $"{minutes:00}:{seconds:00}";
        }

        // Fill Image ������Ʈ(0~1 ������ ��)
        if(timerFillImage != null)
        {
            float fillAmount = currentTime / totalTime;
            timerFillImage.fillAmount = fillAmount;
        }
    }

    private void OnTimerEnd()
    {
        Debug.Log("Ÿ�̸� ����! ���� ����");
        // Ÿ�̸� ���� �� ������ ���� �߰�
        // End State�� �ٷ� �̵��ؼ�, ä�� ����(��� ȭ��)
    }

    // �ܺο��� ���� ���� �ð��� Ȯ���� �� �ִ� ������Ƽ
    public float RemainingTime => currentTime;
    public bool IsTimerRunning => isTimerRunning;
#endregion





    private void HandlePouring()
    {
        Vector3 tilt = GetSimulatedAcceleration(); // tilt: ����̴�

        // ���� ����
        bool isTilting = tilt.x > 0.3f || tilt.z > 0.3f;

        if (pouredAmount >= 100f)
        {
            pourIntensity = 0f;
            UpdatePouringAnimation(0);
            return;
        }

        if(isTilting)
        {
            // ������ ������ ���� ���� ���� ����, Mathf.Clmap01: �־��� ���ڸ� 0�� 1 ���̿� �����ִ� �Լ�
            pourIntensity = Mathf.Clamp01(Mathf.Max(Mathf.Abs(tilt.x), Mathf.Abs(tilt.z))); // ���� �κ� �� �𸣰���
            pouredAmount += Time.deltaTime * pourIntensity * pourSpeed;
        }
        else
        {
            // ���� �پ��ٰ� ����, Mathf.MovToWards: ������ ��(current)�� ��ǥ ��(target)���� ������ �ӵ��� �̵���Ű�� �Լ�.
            pourIntensity = Mathf.MoveTowards(pourIntensity, 0f, Time.deltaTime * pourDecreaseSpeed);
        }

        pouredAmount = Mathf.Min(pouredAmount, 100f); // 100���� ���� (capped)
        UpdatePouringUI(pouredAmount);
        UpdatePouringAnimation(pourIntensity);
        
    }

    private Vector3 GetSimulatedAcceleration()
    {
#if UNITY_EDITOR
        float xInput = 0f;
        float zInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) xInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) xInput = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) zInput = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) zInput = -1f;

        // ������ �ִ� �ð��� ���� ���� ���� (Time.deltaTime ����)
        simulatedTilt.x = Mathf.Clamp(simulatedTilt.x + (xInput * simulatedTiltSpeed * Time.deltaTime), -simulatedTiltMax, simulatedTiltMax);
        simulatedTilt.y = Mathf.Clamp(simulatedTilt.y + zInput * simulatedTiltSpeed * Time.deltaTime, -simulatedTiltMax, simulatedTiltMax);

        // Ű�� ���� ���Ⱑ õõ�� ������
        if (xInput == 0) simulatedTilt.x = Mathf.MoveTowards(simulatedTilt.x, 0f, simulatedTiltSpeed * Time.deltaTime);
        if (zInput == 0) simulatedTilt.y = Mathf.MoveTowards(simulatedTilt.y, 0f, simulatedTiltSpeed * Time.deltaTime);

        return new Vector3(simulatedTilt.x, 0f, simulatedTilt.y);
#else
    return Input.acceleration;
#endif
    }

    public void SetState(CoffeeState newState)
    {
        currentState = newState;
        SetAllPanelsInactive();
        basePanel.SetActive(newState == CoffeeState.BaseSelect);
        pouringPanel.SetActive(newState == CoffeeState.Pouring);
        syrupPanel.SetActive(newState == CoffeeState.Syrup);

        // ���º� �ʱ�ȭ�� ���⿡ �־��� �� ����
        switch(newState)
        {
            case CoffeeState.BaseSelect:
                InitBaseSelect();
                break;
            case CoffeeState.Pouring:
                InitPouring();
                break;
            case CoffeeState.Syrup:
                InitSyrup();
                break;
        }
    }

    void InitBaseSelect()
    {
        selectedBase = "";
    }

    void InitPouring() 
    {
        pouredAmount = 0f;
        pourIntensity = 0f;

        // selectedBase�� ���� �̹��� �̸� �ٲٱ�
        pourDrink.sprite = baseSprites[selectedBase];
        UpdatePouringUI(pouredAmount);

        // selectedBase

        ResetAllBools(pouringAnimator);
        if (selectedBase.Contains("Water"))
            pouringAnimator.SetBool("Water", true);
        else if(selectedBase.Contains("Milk"))
            pouringAnimator.SetBool("Milk", true);

        UpdatePouringAnimation(0);

    }
    void InitSyrup()
    {
        pumpingCooldown = 0f;
        
        // ���� ���� �ڷ�ƾ�� �ִٸ� �ߴ�
        if(returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        // �ӱ� �߾����� �ʱ�ȭ
        mug.GetComponent<RectTransform>().anchoredPosition = mugDefaultPosition;

        // ���������� ����� �÷� �ʱ�ȭ
        lastUsedSyrup = "";

    }
    private void SetAllPanelsInactive()
    {
        basePanel?.SetActive(false);
        pouringPanel?.SetActive(false);
        syrupPanel?.SetActive(false);
    }
    private void ResetAllBools(Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(param.name, false);
            }
        }
    }

    public void OnBaseSelected(string baseName)
    {
        selectedBase = baseName;
        SetState(CoffeeState.Pouring);
    }
    public void OnNextToBaseSelect() => SetState(CoffeeState.BaseSelect);
    public void OnNextToPouring() => SetState(CoffeeState.Pouring);
    public void OnNextToSyrup() => SetState(CoffeeState.Syrup);
    public void OnNextToWhippedCream() => SetState(CoffeeState.WhippedCream);

    public void OnSyrupButtonClick(string syrupName)
    {
        if (pumpingCooldown > 0) return;
        pumpingCooldown = pumpingCooltime;
        Animator anim = EventSystem.current.currentSelectedGameObject.GetComponent<Animator>(); 
        if (anim == null) return;

        // �ִϸ��̼� ����
        anim.SetTrigger("Pump");

        // �÷� ī��Ʈ �߰�
        if (!syrupCounts.ContainsKey(syrupName))
            syrupCounts[syrupName] = 1;
        else
            syrupCounts[syrupName]++;

        UpdateSyrupUI(syrupCounts[syrupName], anim.transform);

        // ���� ���� ���� ���� �ڷ�ƾ�� �ִٸ� �ߴ�
        if(returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
        
        // �ٸ� �÷����� �ٲ���ų� ó�� �÷��� ����ϴ� ��쿡�� ��ġ �̵�
        if(lastUsedSyrup != syrupName)
        {
            MoveMugToPumpPosition(anim.GetComponent<RectTransform>());
            lastUsedSyrup = syrupName;
        }

        // �÷� ���� �� ���� �ð� �� �ӱ׸� �⺻ ��ġ�� �ǵ�����
        returnCoroutine = StartCoroutine(ReturnMugToDefaultPosition());

    }

    // �ӱ׸� �⺻ ��ġ�� �ǵ����� �ڷ�ƾ �߰�
    private IEnumerator ReturnMugToDefaultPosition()
    {
        // �÷� ���� �ִϸ��̼��� ���� �ð� + �߰� ��� �ð� 
        yield return new WaitForSeconds(1.5f);

        // �ӱ׸� �⺻ ��ġ�� �ε巴�� �̵�
        RectTransform mugRect = mug.GetComponent<RectTransform>();
        Vector2 startPos = mugRect.anchoredPosition;
        Vector2 targetPos = mugDefaultPosition;

        float duration = 0.5f; // �̵��� �ɸ� �ð�
        float elasped = 0f;

        while(elasped < duration)
        {
            elasped += Time.deltaTime;
            float t = elasped / duration; // ���� Ŀ��

            // �ε巯�� �̵��� ���� easing �Լ� ����
            t = Mathf.SmoothStep(0f, 1f, t);

            mugRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ�� ���� ����
        mugRect.anchoredPosition = mugDefaultPosition;

        // ������ ����� �÷� �ʱ�ȭ (�⺻ ��ġ�� ���ƿ����Ƿ�)
        lastUsedSyrup = "";
        returnCoroutine = null;
    }

    private void MoveMugToPumpPosition(RectTransform syrupTransform)
    {
        RectTransform mugRect = mug.GetComponent<RectTransform>();

        // 1. �÷� ��ư�� ���� ��ġ ��������
        Vector3 syrupWorldPos = syrupTransform.position;

        // 2. �ӱ��� �θ� Canvas�� �������� ���� ��ǥ ��ȯ
        RectTransform canvasRect = syrupPanel.GetComponent<RectTransform>();

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            syrupWorldPos,
            null, // UI Camera�� ���� ��� null ���
            out localPoint))
        {
            // 3. X ��ġ�� �÷� ��ư ����, Y�� �⺻ ��ġ ����
            Vector2 targetPos = new Vector2(localPoint.x + mugOffset.x, mugDefaultPosition.y + mugOffset.y);
            mugRect.anchoredPosition = targetPos;

            Debug.Log($"��ȯ�� ���� ����Ʈ: {localPoint}, ���� ��ġ: {targetPos}");
        }
    }



    private void CheckRecipe() // ������ ���� ScriptableObject�� �����
    {
        //if(selectedBase == "Milk" && selectedSyrups.Contains("ī���"))
        //{
        //    ShowResult("ī����� �ϼ�!");
        //}
        //else
        //{
        //    ShowResult("�����ǰ� �޶��!");
        //}
    }

    private void UpdatePouringAnimation(float intensity)
    {
        // 5�ܰ� (0~4)�� ���� ������ ����
        int level = 0;

        if (intensity > 0.9f)
            level = 4;
        else if (intensity > 0.7f)
            level = 3;
        else if (intensity > 0.5f)
            level = 2;
        else if (intensity > 0.3f)
            level = 1;
        else
            level = 0;

        pouringAnimator.SetInteger("PourLevel", level);
    }

    private void UpdatePouringUI(float amount)
    {
        if(amountText != null)
            amountText.text = $"{amount:F2} ml";
    }

    private void UpdateSyrupUI(int count, Transform targetTransform) // Ƚ��, targetTransform
    {
        if (syrupLabelPrefab == null) return;

        GameObject label = Instantiate(syrupLabelPrefab, syrupPanel.transform);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
        RectTransform parentRect = syrupPanel.GetComponent<RectTransform>();
        RectTransform labelRect = label.GetComponent<RectTransform>();

        Vector2 localPoint; 
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, Camera.main, out localPoint))
        {
            localPoint.y += textOffset;

            labelRect.anchoredPosition = localPoint;
        }

        TextMeshProUGUI text = label.GetComponent<TextMeshProUGUI>();
        if (text != null)
            text.text = $"{count}";

        // ȿ�� ������Ʈ �ڵ� ����
        SyrupLabelEffect effect = label.GetComponent<SyrupLabelEffect>();
        if (effect != null)
            effect.Play();
    }

}
