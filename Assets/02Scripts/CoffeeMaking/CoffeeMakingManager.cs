using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public enum CoffeeState { BaseSelect, Pouring, Syrup, WhippedCreamSelect, WhippedCreamSqueeze }

[System.Serializable]
public class BaseSpriteEntry // entry: 항목
{
    public string baseName;
    public Sprite sprite;
}
[System.Serializable]
public class WhippingGasSpriteEntry
{
    public string whippingGasName;
    public Sprite sprite;
}
[System.Serializable]
public class WhippedCreamSpriteEntry
{
    public string levelName;
    public Sprite sprite;
}
public class CoffeeMakingManager : MonoBehaviour
{
    [SerializeField] private GameObject basePanel;
    [SerializeField] private GameObject pouringPanel;
    [SerializeField] private GameObject syrupPanel;
    [SerializeField] private GameObject whippedCreamSelectPanel;
    [SerializeField] private GameObject whippedCreamSqueezePanel;

    private CoffeeState currentState;
    private string selectedBase;
    [SerializeField] private BaseSpriteEntry[] baseSpriteEntries;
    private Dictionary<string, Sprite> baseSprites;

    [SerializeField] private Image pourDrink;
    [SerializeField] private Animator pouringAnimator; // 애니메이션 컨트롤용
    [SerializeField] private float pourSpeed = 10f;
    [SerializeField] private float pourDecreaseSpeed = 5f;
    [SerializeField] private TextMeshProUGUI amountText;

    private float pouredAmount = 0f; // 현재 따라진 양
    private float pourIntensity = 0f; // 기울기 기반으로 계산된 양
    private Vector2 simulatedTilt = Vector2.zero;
    [SerializeField] private float simulatedTiltSpeed = 1.5f;
    [SerializeField] private float simulatedTiltMax = 1.2f;


    // 시럽 펌핑 횟수 저장용 딕셔너리
    private Dictionary<string, int> syrupCounts = new();
    [SerializeField] private HorizontalLayoutGroup syrupListPanel;
    [SerializeField] private GameObject syrupLabelPrefab; // 시럽이 추가되었음을 나타낼 UI 프리팹 (Text)
    [SerializeField] private float textOffset;
    [SerializeField] Transform mug;
    [SerializeField] private Vector3 mugDefaultPosition; // 중앙 위치
    [SerializeField] private Vector3 mugOffset = new Vector3(-50f, 0f, 0f); // 디스펜서 왼쪽으로 이동할 오프셋
    private string lastUsedSyrup = ""; // 마지막으로 사용한 시럽 이름 추적
    private Coroutine returnCoroutine; // 현재 실행 중인 복귀 코루틴 참조
    private float pumpingCooldown;
    [SerializeField] float pumpingCooltime = 0.5f;

    // 휘핑가스 선택
    private string selectedWhippingGas;
    [SerializeField] private WhippingGasSpriteEntry[] whippingGasSpriteEntries;
    private Dictionary<string, Sprite> whippingGasSprites;
    [SerializeField] private Image squeezeWhippingGas;

    // 휘핑크림 게이지 시스템
    [Header("Whipped Cream Gauge System")]
    [SerializeField] private Image whippingAmountFillImage;
    [SerializeField] private TextMeshProUGUI whippingAmountText;
    [SerializeField] private Image whippedCreamImage;
    [SerializeField] private TextMeshProUGUI startOrStopText;
    [SerializeField] private RectTransform lowArrow;
    [SerializeField] private RectTransform highArrow;
    [SerializeField] private RectTransform veryHighArrow;
    [SerializeField] private WhippedCreamSpriteEntry[] whippedCreamSprites;
    [SerializeField] private Sprite noneWhippedCreamSpirte;
    [SerializeField] private float whippingSpeed = 1f;

    private Dictionary<string, Sprite> whippedCreamSpriteDict;
    private bool isWhipping = false;
    private float currentWhippingAmount = 0f;


    // 타이머 관련 변수들
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private Image timerFillImage;
    [SerializeField] private float totalTime = 120f; // 총 시간(초)
    [SerializeField] private float fillSmoothSpeed = 2f; // Fill Image 부드러운 감소 속도
    private float currentTime;
    private float targetFillAmount; // 목표 Fill Amount
    private bool isTimerRunning = false;
    private Coroutine timerCoroutine;

    private void Start()
    {
        baseSprites = new Dictionary<string, Sprite>();
        foreach(var entry in baseSpriteEntries)
        {
            baseSprites[entry.baseName] = entry.sprite;
        }

        whippingGasSprites = new Dictionary<string, Sprite>();
        foreach (var entry in whippingGasSpriteEntries)
        {
            whippingGasSprites[entry.whippingGasName] = entry.sprite;
        }

        whippedCreamSpriteDict = new Dictionary<string, Sprite>();
        foreach(var entry in whippedCreamSprites)
        {
            whippedCreamSpriteDict[entry.levelName] = entry.sprite;
        }

        InitializeTimer();
        StartTimer();
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
        if (currentState == CoffeeState.WhippedCreamSqueeze)
        {
            HandleWhipping();
        }

        // Fill Image 부드러운 애니메이션 처리
        if (timerFillImage != null && isTimerRunning)
        {
            SmoothUpdateFillImage();
        }
    }

    private void HandleWhipping()
    {
        if(isWhipping && currentWhippingAmount < 1f)
        {
            currentWhippingAmount += Time.deltaTime * whippingSpeed;
            currentWhippingAmount = Mathf.Clamp01(currentWhippingAmount);

            UpdateWhippingGauge();
        }
    }
    private void UpdateWhippingDisplay(string text, string spriteKey)
    {
        if (whippingAmountText != null)
        {
            whippingAmountText.text = text;
        }

        if (whippedCreamImage != null && whippedCreamSpriteDict.ContainsKey(spriteKey)) // 키가 있다면
        {
            whippedCreamImage.sprite = whippedCreamSpriteDict[spriteKey];
        }
    }
    private void UpdateWhippingGauge()
    {
        // Fill Image 업데이트
        if (whippingAmountFillImage != null)
        {
            whippingAmountFillImage.fillAmount = currentWhippingAmount;
        }

        // 현재 Fill Amount를 기준으로 화살표 위치와 비교
        float fillImageWidth = whippingAmountFillImage.rectTransform.rect.width;
        float currentFillPosition = currentWhippingAmount * fillImageWidth; // 현재 채워진 위치

        // 화살표들의 상대적 위치 계산 (Fill Image 기준)
        float lowArrowPos = GetArrowRelativePosition(lowArrow, whippingAmountFillImage.rectTransform);
        float highArrowPos = GetArrowRelativePosition(highArrow, whippingAmountFillImage.rectTransform);
        float veryHighArrowPos = GetArrowRelativePosition(veryHighArrow, whippingAmountFillImage.rectTransform);

        // 텍스트와 이미지 업데이트
        if (currentFillPosition >= veryHighArrowPos)
        {
            UpdateWhippingDisplay("아주 많음", "veryhigh");
        }
        else if (currentFillPosition >= highArrowPos)
        {
            UpdateWhippingDisplay("많음", "high");
        }
        else if (currentFillPosition >= lowArrowPos)
        {
            UpdateWhippingDisplay("적음", "low");
        }
        else if (currentWhippingAmount > 0)
        {
            UpdateWhippingDisplay("아주 적음", "verylow");
        }
    }

    private float GetArrowRelativePosition(RectTransform arrow, RectTransform fillImage)
    {
        // 화살표의 월드 위치를 Fill Image의 로컬 좌표계로 변환
        Vector3[] arrowCorners = new Vector3[4];
        Vector3[] fillCorners = new Vector3[4];

        // GetWorldCorners: RectTransform의 네 꼭짓점의 월드 좌표를 반환하는 함수
        arrow.GetWorldCorners(arrowCorners);
        fillImage.GetWorldCorners(fillCorners);

        // Fill Image의 왼쪽 끝과 화살표 위치의 차이를 계산
        float fillImageLeft = fillCorners[0].x;
        float arrowCenterX = (arrowCorners[0].x + arrowCorners[2].x) / 2f; // 화살표 중심 X 위치

        return arrowCenterX - fillImageLeft;
    }


    #region Timer Methods
    private void InitializeTimer()
    {
        currentTime = totalTime;
        targetFillAmount = 0f; // 초기 목표값 0으로 설정
        UpdateTimerUI();

        if(timerFillImage != null)
        {
            timerFillImage.type = Image.Type.Filled;
            timerFillImage.fillMethod = Image.FillMethod.Horizontal;
            timerFillImage.fillOrigin = 0; // 왼쪽에서 시작(Left)
            timerFillImage.fillAmount = 1f; // 시작할 때는 가득 참
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
        targetFillAmount = 0f; // 목표값도 리셋
        UpdateTimerUI();
    }

    private IEnumerator TimerCountdown()
    {
        while(currentTime > 0 && isTimerRunning)
        {
            yield return new WaitForSeconds(1f); // 1초 대기

            currentTime -= 1f;
            currentTime = Mathf.Max(0f, currentTime); // 0 이하로 내려가지 않도록

            UpdateTimerUI();
        }

        // 타이머가 끝났을 때
        if(currentTime <= 0)
        {
            OnTimerEnd();
        }

        isTimerRunning = false;
        timerCoroutine = null;
    }

    private void UpdateTimerUI()
    {
        // 텍스트 업데이트 (분:초 형식)
        if(timeRemainingText != null)
        {
            int seconds = Mathf.FloorToInt(currentTime);
            timeRemainingText.text = seconds.ToString();
        }
        
        // 목표 Fill Amount 업데이트(실제 Fill Image는 SmoothUpdateFillImage에서 처리)
        float fillAmount = currentTime / totalTime;
    }

    private void SmoothUpdateFillImage()
    {
        if (timerFillImage == null) return;

        // 현재 fillAmount를 목표값으로 부드럽게 이동
        float currentFillAmount = timerFillImage.fillAmount;
        float newFillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, fillSmoothSpeed * Time.deltaTime);

        timerFillImage.fillAmount = newFillAmount;
    }

    private void OnTimerEnd()
    {
        Debug.Log("타이머 종료! 게임 종료");
        // 타이머 종료 시 실행할 로직 추가
        // End State로 바로 이동해서, 채점 받음(결과 화면)
    }

    // 외부에서 현재 남은 시간을 확인할 수 있는 프로퍼티
    public float RemainingTime => currentTime;
    public bool IsTimerRunning => isTimerRunning;
#endregion


    private void HandlePouring()
    {
        Vector3 tilt = GetSimulatedAcceleration(); // tilt: 기울이다

        // 기울기 감지
        bool isTilting = tilt.x > 0.3f || tilt.z > 0.3f;

        if (pouredAmount >= 100f)
        {
            pourIntensity = 0f;
            UpdatePouringAnimation(0);
            return;
        }

        if(isTilting)
        {
            // 기울어진 정도에 따라 기울기 강도 조절, Mathf.Clmap01: 주어진 숫자를 0과 1 사이에 묶어주는 함수
            pourIntensity = Mathf.Clamp01(Mathf.Max(Mathf.Abs(tilt.x), Mathf.Abs(tilt.z))); // 여기 부분 잘 모르겠음
            pouredAmount += Time.deltaTime * pourIntensity * pourSpeed;
        }
        else
        {
            // 점점 줄어들다가 멈춤, Mathf.MovToWards: 지정된 값(current)을 목표 값(target)으로 일정한 속도로 이동시키는 함수.
            pourIntensity = Mathf.MoveTowards(pourIntensity, 0f, Time.deltaTime * pourDecreaseSpeed);
        }

        pouredAmount = Mathf.Min(pouredAmount, 100f); // 100으로 제한 (capped)
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

        // 누르고 있는 시간에 따라 기울기 누적 (Time.deltaTime 적용)
        simulatedTilt.x = Mathf.Clamp(simulatedTilt.x + (xInput * simulatedTiltSpeed * Time.deltaTime), -simulatedTiltMax, simulatedTiltMax);
        simulatedTilt.y = Mathf.Clamp(simulatedTilt.y + zInput * simulatedTiltSpeed * Time.deltaTime, -simulatedTiltMax, simulatedTiltMax);

        // 키를 떼면 기울기가 천천히 복원됨
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
        whippedCreamSelectPanel.SetActive(newState == CoffeeState.WhippedCreamSelect);
        whippedCreamSqueezePanel.SetActive(newState == CoffeeState.WhippedCreamSqueeze);

        // 상태별 초기화도 여기에 넣어줄 수 있음
        switch (newState)
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
            case CoffeeState.WhippedCreamSelect:
                // 휘핑크림 선택 패널
                InitWhippedCreamSelect();
                break;
            case CoffeeState.WhippedCreamSqueeze:
                InitWhippedCreamSqueeze();
                break;
        }
    }

    private void InitBaseSelect()
    {
        selectedBase = "";
    }

    private void InitPouring() 
    {
        pouredAmount = 0f;
        pourIntensity = 0f;

        // selectedBase에 따라서 이미지 이름 바꾸기
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
    private void InitSyrup()
    {
        pumpingCooldown = 0f;
        
        // 기존 복귀 코루틴이 있다면 중단
        if(returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        // 머그 중앙으로 초기화
        mug.GetComponent<RectTransform>().anchoredPosition = mugDefaultPosition;

        // 마지막으로 사용한 시럽 초기화
        lastUsedSyrup = "";

    }

    private void InitWhippedCreamSelect()
    {
        selectedWhippingGas = "";
    }

    private void InitWhippedCreamSqueeze()
    {
        // selectedWhippingGas에 따라서 이미지 이름 바꾸기
        squeezeWhippingGas.sprite = whippingGasSprites[selectedWhippingGas];

        // 휘핑크림 게이지 시스템 초기화
        currentWhippingAmount = 0f;
        isWhipping = false;

        // Fill Image 초기화(0으로 설정)
        if(whippingAmountFillImage != null)
        {
            whippingAmountFillImage.fillAmount = 0f;
        }

        // 텍스트 초기화("아주 적음"으로 설정)
        if(whippingAmountText != null)
        {
            whippingAmountText.text = "아주 적음";
        }

        // 휘핑 크림 이미지를 none으로 초기화
        if(whippedCreamImage != null)
        {
            whippedCreamImage.sprite = noneWhippedCreamSpirte;
        }

        // 시작&멈춤 버튼 텍스트를 "시작"으로 초기화
        if(startOrStopText != null)
        {
            startOrStopText.text = "시작";
        }
    }

    private void SetAllPanelsInactive()
    {
        basePanel?.SetActive(false);
        pouringPanel?.SetActive(false);
        syrupPanel?.SetActive(false);
        whippedCreamSelectPanel?.SetActive(false);
        whippedCreamSqueezePanel?.SetActive(false);

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
    public void OnNextToWhippedCreamSelect() => SetState(CoffeeState.WhippedCreamSelect);

    public void OnSyrupButtonClick(string syrupName)
    {
        if (pumpingCooldown > 0) return;
        pumpingCooldown = pumpingCooltime;
        Animator anim = EventSystem.current.currentSelectedGameObject.GetComponent<Animator>(); 
        if (anim == null) return;

        // 애니메이션 실행
        anim.SetTrigger("Pump");

        // 시럽 카운트 추가
        if (!syrupCounts.ContainsKey(syrupName))
            syrupCounts[syrupName] = 1;
        else
            syrupCounts[syrupName]++;

        UpdateSyrupUI(syrupCounts[syrupName], anim.transform);

        // 현재 실행 중인 복귀 코루틴이 있다면 중단
        if(returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
        
        // 다른 시럽으로 바뀌었거나 처음 시럽을 사용하는 경우에만 위치 이동
        if(lastUsedSyrup != syrupName)
        {
            MoveMugToPumpPosition(anim.GetComponent<RectTransform>());
            lastUsedSyrup = syrupName;
        }

        // 시럽 펌핑 후 일정 시간 뒤 머그를 기본 위치로 되돌리기
        returnCoroutine = StartCoroutine(ReturnMugToDefaultPosition());

    }

    // 머그를 기본 위치로 되돌리는 코루틴 추가
    private IEnumerator ReturnMugToDefaultPosition()
    {
        // 시럽 펌핑 애니메이션이 끝날 시간 + 추가 대기 시간 
        yield return new WaitForSeconds(1.5f);

        // 머그를 기본 위치로 부드럽게 이동
        RectTransform mugRect = mug.GetComponent<RectTransform>();
        Vector2 startPos = mugRect.anchoredPosition;
        Vector2 targetPos = mugDefaultPosition;

        float duration = 0.5f; // 이동에 걸릴 시간
        float elasped = 0f;

        while(elasped < duration)
        {
            elasped += Time.deltaTime;
            float t = elasped / duration; // 점점 커짐

            // 부드러운 이동을 위한 easing 함수 적용
            t = Mathf.SmoothStep(0f, 1f, t);

            mugRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // 정확한 위치로 최종 설정
        mugRect.anchoredPosition = mugDefaultPosition;

        // 마지막 사용한 시럽 초기화 (기본 위치로 돌아왔으므로)
        lastUsedSyrup = "";
        returnCoroutine = null;
    }

    private void MoveMugToPumpPosition(RectTransform syrupTransform)
    {
        RectTransform mugRect = mug.GetComponent<RectTransform>();

        // 1. 시럽 버튼의 월드 위치 가져오기
        Vector3 syrupWorldPos = syrupTransform.position;

        // 2. 머그의 부모 Canvas를 기준으로 로컬 좌표 변환
        RectTransform canvasRect = syrupPanel.GetComponent<RectTransform>();

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            syrupWorldPos,
            null, // UI Camera가 없는 경우 null 사용
            out localPoint))
        {
            // 3. X 위치는 시럽 버튼 기준, Y는 기본 위치 유지
            Vector2 targetPos = new Vector2(localPoint.x + mugOffset.x, mugDefaultPosition.y + mugOffset.y);
            mugRect.anchoredPosition = targetPos;

            Debug.Log($"변환된 로컬 포인트: {localPoint}, 최종 위치: {targetPos}");
        }
    }
    public void OnWhippedCreamSelected(string whippedCreamName)
    {
        selectedWhippingGas = whippedCreamName;
        SetState(CoffeeState.WhippedCreamSqueeze);
    }

    public void OnAdjustmentButtonClick()
    {
        if(startOrStopText.text == "시작")
        {
            // 시작 상태로 변경
            startOrStopText.text = "멈춤";
            isWhipping = true;
            // 다음 state로 넘기기(결과?)
        }
        else
        {
            // 멈춤 상태로 변경
            isWhipping = false;
        }
    }

    private void CheckRecipe() // 레시피 조건 ScriptableObject로 만들기
    {
        //if(selectedBase == "Milk" && selectedSyrups.Contains("카라멜"))
        //{
        //    ShowResult("카라멜라뗴 완성!");
        //}
        //else
        //{
        //    ShowResult("레시피가 달라요!");
        //}
    }

    private void UpdatePouringAnimation(float intensity)
    {
        // 5단계 (0~4)로 기울기 정도를 나눔
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

    private void UpdateSyrupUI(int count, Transform targetTransform) // 횟수, targetTransform
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

        // 효과 컴포넌트 자동 실행
        SyrupLabelEffect effect = label.GetComponent<SyrupLabelEffect>();
        if (effect != null)
            effect.Play();
    }

}
