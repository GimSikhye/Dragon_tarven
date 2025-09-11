using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;

public enum CoffeeState { BaseSelect, DoTheShot, BasePouring, SyrupPumping, WhippedCreamSelect, WhippedCreamSqueeze }
// 점수에 따른 점수 아이콘 이미지


[System.Serializable]
public class BaseSpriteEntry // 재료 Sprite 항목
{
    public string baseName;
    public Sprite sprite;
}
[System.Serializable]
public class WhippingGasSpriteEntry // 휘핑가스 Sprite 항목
{
    public string whippingGasName;
    public Sprite sprite;
}
[System.Serializable]
public class WhippedCreamSpriteEntry // 휘핑크림 Sprite 항목(레벨에 따라)
{
    public string levelName; 
    public Sprite sprite;
}
public class CoffeeMakingManager : MonoBehaviour
{
    [Header("단계별 Panel")]
    [SerializeField] private GameObject baseSelectPanel;
    [SerializeField] private GameObject shotPanel;
    [SerializeField] private GameObject basePouringPanel;
    [SerializeField] private GameObject syrupPumpingPanel;
    [SerializeField] private GameObject whippingGasSelectPanel;
    [SerializeField] private GameObject whippedCreamSqueezePanel;

    [Header("오답 노트 UI")]
    [SerializeField] private RectTransform commentNoteLineParent; // 오답 노트 UI가 붙은 패널
    [SerializeField] private GameObject blueSpeechBubblePrefab;
    [SerializeField] private GameObject redSpeechBubblePrefab;
    [SerializeField] private GameObject commentTextLine; // TextMeshProUGUI 한 줄짜리 프리팹
    [SerializeField] private float speechBubbleOffsetX = 150f;

    private void DisplayPourAmountFeedbackBubble(float difference)
    {
        GameObject prefabToUse = difference > 0 ? blueSpeechBubblePrefab : redSpeechBubblePrefab; // 더 많다면, 빨간 말풍선을 띄우고 더 적게 따르라고 코멘트.
        string textToShow = difference > 0 ? "더 적게" : "더 많이";

        // 찾은 마지막 텍스트 UI (노트 마지막 줄)
        TextMeshProUGUI[] texts = commentNoteLineParent.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length == 0) return;

        var lastText = texts.Last(); // 배열의 마지막 요소를 가져오는 LINQ 메소드
        if (lastText == null) return;

        // 말풍선 생성
        GameObject speechBubble = Instantiate(prefabToUse, commentNoteLineParent);
        speechBubble.GetComponentInChildren<TextMeshProUGUI>().text = textToShow;

        // 말풍선을 텍스트 옆에 배치 (X 오프셋만 적용)
        RectTransform speechBubbleRect = speechBubble.GetComponent<RectTransform>();
        RectTransform textRect = lastText.GetComponent<RectTransform>();

        speechBubbleRect.anchoredPosition = textRect.anchoredPosition + new Vector2(speechBubbleOffsetX, 0f); // 오른쪽에 배치

    }

    private CoffeeState currentState; // 현재 상태
    private string selectedBase; // 선택된 재료

    [Header("저장할 Sprite들")]
    [SerializeField] private BaseSpriteEntry[] baseSpriteEntries;
    private Dictionary<string, Sprite> baseSprites;
    [SerializeField] private WhippingGasSpriteEntry[] whippingGasSpriteEntries;
    private Dictionary<string, Sprite> whippingGasSprites;
    [SerializeField] private WhippedCreamSpriteEntry[] whippedCreamSpriteEntries;
    private Dictionary<string, Sprite> whippedCreamSprites;

    // Shot 관련 변수
    [Header("Shot System")]
    [SerializeField] private Animator[] outletAnimators; // 배출구 1, 2, 3, 4의 애니메이터들
    [SerializeField] private Button[] shotButtons; // Shot 버튼들 배열
    [SerializeField] private Transform[] shotGlasses; // 샷잔들의 Transform
    [SerializeField] private Transform shotMugTransform; // Mug의 Transform
    [SerializeField] private float mugDetectionRadius = 150f; // Mug 근처 감지 반경(드래그로 샷잔을 머그에 옮길때)

    private readonly Color defaultShotButtonColor = new Color(1f, 172f / 255f, 65f / 255f, 1f);
    private readonly Color selectedShotButtonColor = new Color(142f / 255f, 207f / 255f, 40f / 255f, 1f);
    
    private bool[] shotButtonPressed; // 각 버튼이 눌렸는지 추적하는 배열
    private bool[] shotGlassHasShot; // 각 샷잔에 샷이 있는지 여부 
    private bool[] shotGlassPouredToMug; // 각 샷잔이 Mug에 부어졌는지 여부
    private bool[] shotGlassAnimationCompleted; // 각 샷잔의 애니메이션이 완료되었는지 여부
    private bool hasDragStarted = false; // 드래그가 한 번이라도 시작되었는지

    [Header("Base Pour 관련 변수")]
    [SerializeField] private Image pourDrink;
    [SerializeField] private Animator pouringAnimator; // 애니메이션 컨트롤용
    [SerializeField] private float pourSpeed = 10f;
    [SerializeField] private float pourDecreaseSpeed = 5f;
    [SerializeField] private TextMeshProUGUI currentPouredAmountText;

    private float currentPouredAmount = 0f; // 현재 따라진 양
    private float tiltIntensity = 0f; // 기울기 기반으로 계산된 양(기울기 강도)!
    private Vector2 simulatedTilt = Vector2.zero;
    [SerializeField] private float simulatedTiltSpeed = 1.5f;
    [SerializeField] private float simulatedTiltMax = 1.2f; // 왜 titleSpeed값보다 작지?


    [Header("휘핑가스 관련 변수들")]
    private Dictionary<string, int> syrupCounts = new();
    [SerializeField] private HorizontalLayoutGroup syrupListPanel;
    [SerializeField] private GameObject syrupCountLabelPrefab; // 시럽이 추가되었음을 나타낼 UI 프리팹 (Text)
    [SerializeField] private float textOffset;
    [SerializeField] Transform syrupMugTransform;
    [SerializeField] private Vector3 syrupMugDefaultPosition; // 중앙 위치
    [SerializeField] private Vector3 syrupMugOffset = new Vector3(-50f, 0f, 0f); // 디스펜서 왼쪽으로 이동할 오프셋
    private string lastUsedSyrup = ""; // 마지막으로 사용한 시럽 이름 추적
    private Coroutine returnCoroutine; // 현재 실행 중인 복귀 코루틴 참조
    private float pumpingSyrupCooldown;
    [SerializeField] float pumpingCooltime = 0.5f;
    
    // 휘핑가스 선택
    private string selectedWhippingGas;
    [SerializeField] private Image squeezeWhippingGas;

    // 휘핑크림 게이지 시스템
    [Header("휘핑크림 게이지 변수들")]
    [SerializeField] private Image whippedCreamGaugeImage;
    [SerializeField] private TextMeshProUGUI currentWhippingAmountText;
    [SerializeField] private Image currentWhippedCreamImage;
    [SerializeField] private TextMeshProUGUI whippingAmountControlButtonText;
    [SerializeField] private RectTransform whippedCreamGauageLowArrow;
    [SerializeField] private RectTransform whippedCreamGauageHighArrow;
    [SerializeField] private RectTransform whippedCreamGauageveryHighArrow;

    [SerializeField] private Sprite noneWhippedCreamSprite;
    [SerializeField] private float whippingGauageSpeed = 1f;

    private bool isWhipping = false;
    private float currentWhippingAmount = 0f;

    [Header("타이머 세팅 변수들")] /////////
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private Image timerProgressImage;
    [SerializeField] private float totalTime = 120f; // 총 시간(초)
    [SerializeField] private float timerFillSmoothSpeed; // Fill Image 부드러운 감소 속도

    private float currentRemainTime; // 현재 시간
    private float targetFillAmount; // 목표 Fill Amount
    private bool isTimerRunning = false;
    private Coroutine timerCoroutine;

    [Header("오답노트 UI 변수들")]
    [SerializeField] private GameObject resultNotePanel;
    [SerializeField] private Sprite[] scoreIcons;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image scoreImage;


    private void Start()
    {
        baseSprites = new Dictionary<string, Sprite>();
        foreach(var entry in baseSpriteEntries)
        {
            baseSprites[entry.baseName] = entry.sprite;
        }

        // Shot button 상태 배열 초기화
        shotButtonPressed = new bool[shotButtons.Length];
        shotGlassHasShot = new bool[shotGlasses.Length];
        shotGlassPouredToMug = new bool[shotGlasses.Length];
        shotGlassAnimationCompleted = new bool[shotGlasses.Length]; 

        whippingGasSprites = new Dictionary<string, Sprite>();
        foreach (var entry in whippingGasSpriteEntries)
        {
            whippingGasSprites[entry.whippingGasName] = entry.sprite;
        }

        whippedCreamSprites = new Dictionary<string, Sprite>();
        foreach(var entry in whippedCreamSpriteEntries)
        {
            whippedCreamSprites[entry.levelName] = entry.sprite;
        }

        InitializeTimer();
        StartTimer();
        SetState(CoffeeState.BaseSelect);

    }
    private void Update()
    {
        if (currentState == CoffeeState.BasePouring)
        {
            HandleBasePouring();
        }

        if (currentState == CoffeeState.SyrupPumping)
        {
            if (pumpingSyrupCooldown > 0)
                pumpingSyrupCooldown -= Time.deltaTime;
        }
        if (currentState == CoffeeState.WhippedCreamSqueeze)
        {
            HandleWhipping();
        }

        // Fill Image 부드러운 애니메이션 처리
        if (timerProgressImage != null && isTimerRunning)
        {
            SmoothTimerFillAnimation();
        }
    }

   
    public bool CanDragShotGlass(int shotGlassNumber)
    {
        if (currentState != CoffeeState.DoTheShot) return false;
        int index = shotGlassNumber - 1;
        return shotGlassHasShot[index] && !shotGlassPouredToMug[index];
    }

    public void OnShotGlassDragStart()
    {
        hasDragStarted = true;
    }

    public bool IsNearMug(Vector3 shotGlassPosition)
    {
        if (shotMugTransform == null) return false;

        float distance = Vector3.Distance(shotGlassPosition, shotMugTransform.position);
        return distance <= mugDetectionRadius;
    }

    public void PourShotToMug(int shotGlassNumber, ShotGlassDragHandler dragHandler)
    {
        int index = shotGlassNumber - 1;

        // Mug의 RectTransform을 가져옴
        RectTransform shotMugRect
            = shotMugTransform.GetComponent<RectTransform>();
        if(shotMugRect == null)
        {
            Debug.LogError("Mug에 RectTransform이 없습니다!");
            return;
        }

        // Mug의 현재 anchoredPosition을 기준으로 붓는 위치 계산
        Vector2 shotMugPosition = shotMugRect.anchoredPosition; // 머그 위치
        Vector2 shotPourPosition = new Vector2(shotMugPosition.x - 250f, shotMugPosition.y + 200f); // 붓는 샷잔의 에스프레소샷

        // 샷잔을 붓는 위치로 이동하고 애니메이션 실행
        dragHandler.MoveToPourPosition(shotPourPosition);

        // 해당 샷잔의 샷이 부어졌다고 체크
        shotGlassPouredToMug[index] = true;

    }

    // 샷잔 애니메이션 완료 시 호출되는 메서드 (ShotGlassDragHandler에서 호출)
    public void OnShotGlassAnimationCompleted(int shotGlassNumber)
    {
        int index = shotGlassNumber - 1;
        shotGlassAnimationCompleted[index] = true;

        Debug.Log($" {shotGlassNumber}번째 샷잔 애니메이션 재생 완료됨");

        // 모든 애니메이션이 완료되었는지 확인
        CheckAllShotsPouredToMug();
    }

    private void CheckAllShotsPouredToMug() // 샷이 있는 모든 샷잔이 Mug에 부어지고 애니메이션이 완료되었는지 확인
    {
        for (int i = 0; i < shotGlassHasShot.Length; i++)
        {
            if (shotGlassHasShot[i]) // 해당 샷잔에 샷이 있다면
            {
                // 샷이 있는 샷잔이 부어지지 않았거나 애니메이션이 완료되지 않았으면 리턴
                if (!shotGlassPouredToMug[i] || !shotGlassAnimationCompleted[i])
                {
                    return;
                }
            }
        }

        // 모든 샷잔이 부어지고 애니메이션이 완료되었으면 다음 단계로 이동
        Debug.Log("모든 샷잔 애니메이션 완료 - 다음 단계로 이동");
        OnNextToPouring();
    }

    public void OnSkipWhippedCream()
    {
        // 휘핑크림 단계들을 모두 건너뜀
        currentWhippingAmount = 0f;

        // 휘핑크림 레벨을 none으로 설정
        if (whippedCreamGaugeImage != null)
            whippedCreamGaugeImage.fillAmount = 0f;

        if (currentWhippingAmountText != null)
            currentWhippingAmountText.text = "없음";

        if (currentWhippedCreamImage != null)
            currentWhippedCreamImage.sprite = noneWhippedCreamSprite;

        isWhipping = false;

        // 다음 단계로 넘어가기
        CheckRecipe(); 
        ShowResultUI();
    }


    private void HandleWhipping() // Whipping 처리 메서드
    {
        if(isWhipping && currentWhippingAmount < 1f)
        {
            currentWhippingAmount += Time.deltaTime * whippingGauageSpeed;
            currentWhippingAmount = Mathf.Clamp01(currentWhippingAmount); // 0~1값 사이

            UpdateWhippingGauge();
        }
    }
    private void UpdateWhippingDisplay(string text, string spriteKey) //
    {
        if (currentWhippingAmountText != null)
        {
            currentWhippingAmountText.text = text;
        }

        if (currentWhippedCreamImage != null && whippedCreamSprites.ContainsKey(spriteKey)) // 키가 있다면
        {
            currentWhippedCreamImage.sprite = whippedCreamSprites[spriteKey];
        }
    }
    private void UpdateWhippingGauge()
    {
        // Fill Image 업데이트
        if (whippedCreamGaugeImage != null)
        {
            whippedCreamGaugeImage.fillAmount = currentWhippingAmount;
        }

        // 현재 Fill Amount를 기준으로 화살표 위치와 비교
        float fillImageWidth = whippedCreamGaugeImage.rectTransform.rect.width;
        float currentFillPosition = currentWhippingAmount * fillImageWidth; // 현재 채워진 위치(픽셀)

        float lowArrowPos = GetArrowRelativePosition(whippedCreamGauageLowArrow, whippedCreamGaugeImage.rectTransform);
        float highArrowPos = GetArrowRelativePosition(whippedCreamGauageHighArrow, whippedCreamGaugeImage.rectTransform);
        float veryHighArrowPos = GetArrowRelativePosition(whippedCreamGauageveryHighArrow, whippedCreamGaugeImage.rectTransform);

        // 레벨 결정
        string level = CalculateWhippedLevelFromGauge(currentWhippingAmount); ////


        // 텍스트는 항상 동일
        string displayText = level switch
        {
            "veryhigh" => "아주 많음",
            "high" => "많음",
            "low" => "적음",
            "verylow" => "아주 적음",
            _ => ""
        };

        // 이미지는 Espresso인지에 따라 다르게 처리
        string imageName = 
            selectedWhippingGas == "EspressoWhippingGas" ? $"Espresso_{level}" : level;

        UpdateWhippingDisplay(displayText, imageName);
    }

    private float GetArrowRelativePosition(RectTransform arrow, RectTransform fillImage) // 화살표들의 상대적 위치 계산 (Fill Image 기준)
    {
        // 네 모서리 월드 좌표를 저장할 Vector3 타입의 배열
        Vector3[] arrowVertexs = new Vector3[4]; // 꼭짓점 4개
        Vector3[] fillVertexs = new Vector3[4];

        // GetWorldCorners: RectTransform의 네 꼭짓점의 월드 좌표를 반환하는 함수([0]: 좌측 하단, [1]: 좌측 상단, [2]: 우측 상단, [3]: 우측 하단
        arrow.GetWorldCorners(arrowVertexs);
        fillImage.GetWorldCorners(fillVertexs); // 기준점(fill amount: 0)

        // Fill Image의 왼쪽 끝과 화살표 위치의 차이를 계산
        float fillImageLeft = fillVertexs[0].x; // fillImage 왼쪽 아래 꼭짓점 x좌표
        float arrowCenterX = (arrowVertexs[0].x + arrowVertexs[3].x) / 2f; // 화살표 중심 X 위치

        return arrowCenterX - fillImageLeft;
    }


    #region Timer Methods
    private void InitializeTimer() // 타이머 초기화 함수
    {
        currentRemainTime = totalTime;
        targetFillAmount = 0f; // 목표값 0으로 초기화
        UpdateTimeRemainingText();

        if(timerProgressImage != null)
        {
            //timerProgressImage.type = Image.Type.Filled;
            //timerProgressImage.fillMethod = Image.FillMethod.Horizontal;
            timerProgressImage.fillOrigin = 0; // fillOrigin: 채워지거나 비워지는 방향의 시작점
            timerProgressImage.fillAmount = 1f; // 시작할 때는 가득 참
        }

    }

    public void StartTimer()
    {
        if(!isTimerRunning)
        {
            isTimerRunning = true;
            timerCoroutine = StartCoroutine(TimerCountdown()); // 타이머 카운트 시작
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
        currentRemainTime = totalTime;
        targetFillAmount = 0f; // 목표값도 리셋
        UpdateTimeRemainingText();
    }

    private IEnumerator TimerCountdown()
    {
        while(currentRemainTime > 0 && isTimerRunning) // 남은 시간이 0초보다 크고, 타이머가 진행중이라면
        {
            yield return new WaitForSeconds(1f); // 1초 대기

            currentRemainTime -= 1f;
            currentRemainTime = Mathf.Max(0f, currentRemainTime); // 0 이하로 내려가지 않도록

            UpdateTimeRemainingText();
        }

        // 타이머가 끝났을 때
        if(currentRemainTime <= 0)
        {
            OnTimerEnd();
        }

        isTimerRunning = false;
        timerCoroutine = null;
    }

    private void OnTimerEnd()
    {
        Debug.Log("타이머 종료! 게임 종료");
        // 타이머 종료 시 실행할 로직 추가
        // End State로 바로 이동해서, 채점 받음(결과 화면)
    }

    private void UpdateTimeRemainingText()
    {
        // 남은 시간 텍스트 업데이트
        if(timeRemainingText != null)
        {
            int seconds = Mathf.FloorToInt(currentRemainTime);
            timeRemainingText.text = seconds.ToString();
        }
    }

    private void SmoothTimerFillAnimation()
    {
        if (timerProgressImage == null) return;

        // 현재 fillAmount를 목표값으로 부드럽게 이동
        float timerFillAmount = timerProgressImage.fillAmount;
        float timerFillTargetAmount = Mathf.MoveTowards(timerFillAmount, targetFillAmount, timerFillSmoothSpeed * Time.deltaTime);

        timerProgressImage.fillAmount = timerFillTargetAmount;
    }


    // 외부에서 현재 남은 시간을 확인할 수 있는 프로퍼티
    public float CurrentRemainTime => currentRemainTime;
    public bool IsTimerRunning => isTimerRunning;
    #endregion

    #region Shot System Methods
    // Shot Button 클릭 처리 메서드
    public void OnShotButtonClick(int buttonNumber)
    {
        if (currentState != CoffeeState.DoTheShot) return;
        if (buttonNumber < 1 || buttonNumber > 4) return;

        // 드래그가 시작된 후에는 버튼 클릭 무시
        if (hasDragStarted) return;

        int outletIndex = buttonNumber - 1;

        // 이미 눌린 버튼인지 확인
        if (shotButtonPressed[outletIndex])
        {
            Debug.Log($"Shot Button {buttonNumber}은 이미 눌렸습니다.");
            return; // 이미 눌린 버튼이면 함수 종료
        }

        // 버튼 눌렀으면 true 체크
        shotButtonPressed[outletIndex] = true;

        // 버튼 번호에 해당하는 Outlet 애니메이터 가져오기
        if (outletIndex < outletAnimators.Length && outletAnimators[outletIndex] != null)
        {
            StartCoroutine(PlayBrewAnimation(outletAnimators[outletIndex], buttonNumber));
        }

        // 버튼 색상 변경 및 텍스트 제거
        if (outletIndex < shotButtons.Length && shotButtons[outletIndex] != null)
        {
            // 버튼 색상 변경
            Image buttonImage = shotButtons[outletIndex].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = selectedShotButtonColor;
            }

            // 버튼의 자식 TextMeshProUGUI 텍스트 제거
            TextMeshProUGUI buttonText = shotButtons[outletIndex].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "";
            }

        }
    }

    private IEnumerator PlayBrewAnimation(Animator outletAnimator, int shotGlassNumber)
    {
        // Brew 애니메이션 재생
        outletAnimator.SetTrigger("Brew"); // brew: 양조하다

        // 애니메이터의 상태 변화가 다음 프레임에 완전히 적용되도록 보장
        yield return new WaitForEndOfFrame();

        // "Brew" 애니메이션이 재생 중인지 확인하고 완료까지 대기
        while (outletAnimator.GetCurrentAnimatorStateInfo(0).IsName("Brew") &&
               outletAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) // normalizedTime: Animator 컴포넌트에서 현재 재생 중인 애니메이션 상태의 진행 상황을 0과 1 사이의 값으로 정규화하여 나타내는 속성
        {
            yield return null;
        }

        // 애니메이션 완료 후 None 상태로 변경
        outletAnimator.SetTrigger("None");

        // 애니메이션 완료 후 해당 샷글라스에 샷이 있다고 표시
        int index = shotGlassNumber - 1;
        shotGlassHasShot[index] = true;

        Debug.Log($"Shot Glass {shotGlassNumber}에 샷이 준비되었습니다.");
    }
    #endregion

    private void HandleBasePouring()
    {
        float tiltX = GetSimulatedAcceleration(); // X축 값만 사용

        if (currentPouredAmount >= 100f)
        {
            tiltIntensity = 0f;
            UpdatePouringAnimation(0);
            return;
        }

        if (tiltX < -0.3f) // 왼쪽으로 기울었을 때만
        {
            tiltIntensity = Mathf.Clamp01(Mathf.Abs(tiltX));
            currentPouredAmount += Time.deltaTime * tiltIntensity * pourSpeed;
        }
        else
        {
            // 서서히 줄어듦
            tiltIntensity = Mathf.MoveTowards(tiltIntensity, 0f, Time.deltaTime * pourDecreaseSpeed);
        }

        currentPouredAmount = Mathf.Min(currentPouredAmount, 100f);
        UpdatePouringUI(currentPouredAmount);
        UpdatePouringAnimation(tiltIntensity);
    }

    private float GetSimulatedAcceleration()
    {
        float tiltX = Input.acceleration.x;
        if (Mathf.Abs(tiltX) < 0.1f) tiltX = 0f; // 흔들림 방지
        return tiltX; // 이제 X축 값 그대로 반환
    }


    public void SetState(CoffeeState newState)
    {
        currentState = newState;
        SetAllPanelsInactive();
        baseSelectPanel.SetActive(newState == CoffeeState.BaseSelect);
        shotPanel.SetActive(newState == CoffeeState.DoTheShot);
        basePouringPanel.SetActive(newState == CoffeeState.BasePouring);
        syrupPumpingPanel.SetActive(newState == CoffeeState.SyrupPumping);
        whippingGasSelectPanel.SetActive(newState == CoffeeState.WhippedCreamSelect);
        whippedCreamSqueezePanel.SetActive(newState == CoffeeState.WhippedCreamSqueeze);

        // 상태별 초기화도 여기에 넣어줄 수 있음
        switch (newState)
        {
            case CoffeeState.BaseSelect:
                InitBaseSelect();
                break;
            case CoffeeState.DoTheShot:
                InitDoTheShot();
                break;
            case CoffeeState.BasePouring:
                InitPouring();
                break;
            case CoffeeState.SyrupPumping:
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

    private void InitDoTheShot()
    {
        // 모든 상태 초기화
        for (int i = 0; i < shotButtonPressed.Length; i++)
        {
            shotButtonPressed[i] = false;
            shotGlassHasShot[i] = false;
            shotGlassPouredToMug[i] = false;
        }

        hasDragStarted = false;

        // 모든 Outlet 애니메이터를 None 상태로 초기화
        for (int i = 0; i < outletAnimators.Length; i++)
        {
            if (outletAnimators[i] != null)
            {
                outletAnimators[i].SetTrigger("None");
            }
        }

        // Shot 버튼 상태 초기화
        for(int i = 0; i < shotButtonPressed.Length; i++)
        {
            shotButtonPressed[i] = false;

            // 버튼 UI도 초기 상태로 복원
            Image buttonImage = shotButtons[i].GetComponent<Image>();
            if(buttonImage != null)
            {
                buttonImage.color = defaultShotButtonColor;
            }

            // 버튼 텍스트 복원
            TextMeshProUGUI buttonText = shotButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "TAB"; 
            }
        }
    }

    private void InitPouring() 
    {
        currentPouredAmount = 0f;
        tiltIntensity = 0f;

        // selectedBase에 따라서 이미지 이름 바꾸기
        pourDrink.sprite = baseSprites[selectedBase];
        UpdatePouringUI(currentPouredAmount);

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
        pumpingSyrupCooldown = 0f;
        
        // 기존 복귀 코루틴이 있다면 중단
        if(returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        // 머그 중앙으로 초기화
        syrupMugTransform.GetComponent<RectTransform>().anchoredPosition = syrupMugDefaultPosition;

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
        if(whippedCreamGaugeImage != null)
        {
            whippedCreamGaugeImage.fillAmount = 0f;
        }

        // 텍스트 초기화("아주 적음"으로 설정)
        if(currentWhippingAmountText != null)
        {
            currentWhippingAmountText.text = "아주 적음";
        }

        // 휘핑 크림 이미지를 none으로 초기화
        if(currentWhippedCreamImage != null)
        {
            currentWhippedCreamImage.sprite = noneWhippedCreamSprite;
        }

        // 시작&멈춤 버튼 텍스트를 "시작"으로 초기화
        if(whippingAmountControlButtonText != null)
        {
            whippingAmountControlButtonText.text = "시작";
        }
    }

    private void SetAllPanelsInactive()
    {
        baseSelectPanel?.SetActive(false);
        shotPanel?.SetActive(false);
        basePouringPanel?.SetActive(false);
        syrupPumpingPanel?.SetActive(false);
        whippingGasSelectPanel?.SetActive(false);
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
        SetState(CoffeeState.DoTheShot);
    }

    public void OnNextToBaseSelect() => SetState(CoffeeState.BaseSelect);
    public void OnNextDoTheShot() => SetState(CoffeeState.DoTheShot);
    public void OnNextToPouring() => SetState(CoffeeState.BasePouring);
    public void OnNextToSyrup() => SetState(CoffeeState.SyrupPumping);
    public void OnNextToWhippedCreamSelect() => SetState(CoffeeState.WhippedCreamSelect);

    public void OnSyrupButtonClick(string syrupName)
    {
        if (pumpingSyrupCooldown > 0) return;
        pumpingSyrupCooldown = pumpingCooltime;
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
        RectTransform mugRect = syrupMugTransform.GetComponent<RectTransform>();
        Vector2 startPos = mugRect.anchoredPosition;
        Vector2 targetPos = syrupMugDefaultPosition;

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
        mugRect.anchoredPosition = syrupMugDefaultPosition;

        // 마지막 사용한 시럽 초기화 (기본 위치로 돌아왔으므로)
        lastUsedSyrup = "";
        returnCoroutine = null;
    }

    private void MoveMugToPumpPosition(RectTransform syrupTransform)
    {
        RectTransform mugRect = syrupMugTransform.GetComponent<RectTransform>();

        // 1. 시럽 버튼의 월드 위치 가져오기
        Vector3 syrupWorldPos = syrupTransform.position;

        // 2. 머그의 부모 Canvas를 기준으로 로컬 좌표 변환
        RectTransform canvasRect = syrupPumpingPanel.GetComponent<RectTransform>();

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            syrupWorldPos,
            null, // UI Camera가 없는 경우 null 사용
            out localPoint))
        {
            // 3. X 위치는 시럽 버튼 기준, Y는 기본 위치 유지
            Vector2 targetPos = new Vector2(localPoint.x + syrupMugOffset.x, syrupMugDefaultPosition.y + syrupMugOffset.y);
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
        if(whippingAmountControlButtonText.text == "시작")
        {
            whippingAmountControlButtonText.text = "멈춤";
            isWhipping = true; // 휘핑중
        }
        else
        {
            isWhipping = false;
            CheckRecipe();
        }
    }

    private void CheckRecipe()
    {
        MenuVariantRecipe recipe = OrderData.CurrentRecipe;
        CoffeeResultData result = new CoffeeResultData();

        //  베이스
        result.ShotAccuracy = (selectedBase == recipe.baseType) ? 1f : 0f;

        //  샷 횟수 비교
        int shotCount = shotGlassHasShot.Count(x => x); // 실제 shotCount 측정
        result.ShotAccuracy = (shotCount == recipe.shotCount) ? 1f : 0f; // 샷 정확도

        //  pour량 비교
        float pourError = Mathf.Abs(currentPouredAmount - recipe.expectedPourAmount);
        result.PourAccuracy = 1f - Mathf.Clamp01(pourError / 100f); // 최대 오차 100ml 기준

        //  시럽 비교 (정확히 같은 종류와 횟수만 인정)
        result.SyrupCount = CompareSyrups(recipe.syrups) ? 1 : 0;

        //  휘핑 크림
        result.WhippedLevel = GetCurrentWhippedLevel();

        // 저장
        OrderData.Result = result;

        ShowResultUI();
    }
    private void ShowResultUI()
    {
        resultNotePanel.SetActive(true);

        CoffeeResultData result = OrderData.Result;
        string grade = result.EvaluateGrade();

        feedbackText.text = grade switch
        {
            "Perfect" => "완벽해요! GOOD!",
            "Good" => "좋아요~",
            "Bad" => "조금 아쉬워요...",
            _ => "환불해주세요."
        };

        scoreImage.sprite = grade switch
        {
            "Perfect" => scoreIcons[0],
            "Good" => scoreIcons[1],
            "Bad" => scoreIcons[2],
            _ => scoreIcons[3]
        };

        GenerateNoteLines(result); 
    }

    private IEnumerator ResizeUnderlineToValueOnly(TextMeshProUGUI textComp, Image underlineImg)
    {
        yield return null;

        string fullText = textComp.text;
        int colonIndex = fullText.IndexOf(':');

        // 콜론이 없는 경우: 전체 밑줄 + 시작 X 보정
        if (colonIndex < 0 || colonIndex + 1 >= fullText.Length)
        {
            underlineImg.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            underlineImg.rectTransform.anchorMax = new Vector2(0f, 0.5f);
            underlineImg.rectTransform.pivot = new Vector2(0f, 0.5f);

            float width = textComp.preferredWidth;
            underlineImg.rectTransform.sizeDelta = new Vector2(width, underlineImg.rectTransform.sizeDelta.y);

            // X 보정: 텍스트 자체 위치 고려
            float textStartOffsetX = textComp.margin.x + textComp.fontSize * 1.2f;
            underlineImg.rectTransform.anchoredPosition = new Vector2(textStartOffsetX, 0f);
            yield break;
        }

        // 콜론이 있는 경우
        string before = fullText.Substring(0, colonIndex + 1);
        string after = fullText.Substring(colonIndex + 1).Trim();

        // before 측정용 TMP
        GameObject tmpBeforeGO = new GameObject("TMP_Measure_Before", typeof(RectTransform));
        tmpBeforeGO.transform.SetParent(textComp.transform.parent, false);
        var tmpBefore = tmpBeforeGO.AddComponent<TextMeshProUGUI>();
        tmpBefore.font = textComp.font;
        tmpBefore.fontSize = textComp.fontSize;
        tmpBefore.text = before;
        tmpBefore.alignment = TextAlignmentOptions.Left;
        tmpBefore.enableWordWrapping = false;
        tmpBefore.raycastTarget = false;

        yield return null;
        float startX = tmpBefore.preferredWidth;
        Destroy(tmpBeforeGO);

        // after 측정용 TMP
        GameObject tmpAfterGO = new GameObject("TMP_Measure_After", typeof(RectTransform));
        tmpAfterGO.transform.SetParent(textComp.transform.parent, false);
        var tmpAfter = tmpAfterGO.AddComponent<TextMeshProUGUI>();
        tmpAfter.font = textComp.font;
        tmpAfter.fontSize = textComp.fontSize;
        tmpAfter.text = after;
        tmpAfter.alignment = TextAlignmentOptions.Left;
        tmpAfter.enableWordWrapping = false;
        tmpAfter.raycastTarget = false;

        yield return null;
        float underlineWidth = tmpAfter.preferredWidth;
        Destroy(tmpAfterGO);

        float fontSizeCorrection = textComp.fontSize * 2.5f;
        startX += fontSizeCorrection;

        RectTransform underlineRect = underlineImg.rectTransform;
        underlineRect.anchorMin = new Vector2(0f, 0.5f);
        underlineRect.anchorMax = new Vector2(0f, 0.5f);
        underlineRect.pivot = new Vector2(0f, 0.5f);
        underlineRect.sizeDelta = new Vector2(underlineWidth, underlineRect.sizeDelta.y);
        underlineRect.anchoredPosition = new Vector2(startX, 0f);
    }



    private void GenerateNoteLines(CoffeeResultData result)
    {
        foreach (Transform child in commentNoteLineParent) // 기존 노트 제거
            Destroy(child.gameObject);

        var recipe = OrderData.CurrentRecipe;

        Dictionary<string, string> baseTranslation = new() // 번역
    {
        { "Milk", "우유" },
        { "HotWater", "물" },
        { "Water", "물" }
    };

        // 1. 베이스
        if (selectedBase != recipe.baseType) // 오답이라면
        {
            string baseKor = baseTranslation.FirstOrDefault(str => selectedBase.Contains(str.Key)).Value ?? selectedBase;
            AddNoteLine($"베이스: {baseKor}", underline: true);
        }

        // 2. 샷
        int actualShot = shotGlassHasShot.Count(x => x);
        if (actualShot != recipe.shotCount)
        {
            AddNoteLine($"샷: {actualShot}", underline: true);
        }

        // 3. 용량
        float diff = currentPouredAmount - recipe.expectedPourAmount;
        if (Mathf.Abs(diff) > 5f)
        {
            string label = selectedBase.Contains("Milk") ? "우유량" :
                           selectedBase.Contains("Water") ? "물양" : "용량";

            string content = $"{label}: {currentPouredAmount:F2}";
            AddNoteLine(content, underline: true, pourDiff: diff);
        }

        // 4. 시럽
        if (!CompareSyrups(recipe.syrups))
        {
            AddNoteLine("시럽 종류/횟수가 정확하지 않아요.", underline: true);
        }

        // 5. 휘핑
        if (result.WhippedLevel != recipe.whippedCreamLevel)
        {
            AddNoteLine($"휘핑: {result.WhippedLevel}", underline: true);
        }
    }

    private void AddNoteLine(string text, bool underline = false, float pourDiff = 0f)
    {
        //GameObject lineObj = Instantiate(commentTextLine, commentNoteLineParent);

        //// 자식에 있는 TMP 가져오기
        //TextMeshProUGUI textComp = lineObj.GetComponentInChildren<TextMeshProUGUI>();
        //textComp.text = text;

        //// underline 처리
        //if (underline)
        //{
        //    Transform underlineTr = lineObj.transform.Find("Underline");
        //    if (underlineTr != null && underlineTr.TryGetComponent(out Image underlineImg))
        //    {
        //        StartCoroutine(ResizeUnderlineToValueOnly(textComp, underlineImg));
        //    }
        //}

        //// 말풍선 (우유량 차이 등)
        //if (pourDiff != 0f)
        //{
        //    StartCoroutine(SpawnBubbleNextToText(lineObj.GetComponent<RectTransform>(), pourDiff));
        //}
    }


    private IEnumerator SpawnBubbleNextToText(RectTransform lineRect, float diff)
    {
        yield return null;

        GameObject prefab = diff > 0 ? redSpeechBubblePrefab : blueSpeechBubblePrefab;
        string msg = diff > 0 ? "더 적게" : "더 많이";

        // 말풍선은 NoteLineText의 자식으로 붙이자!
        GameObject bubble = Instantiate(prefab, lineRect);
        RectTransform bubbleRect = bubble.GetComponent<RectTransform>();

        // 텍스트 컴포넌트 가져오기
        if (lineRect == null)
        {
            Debug.Log("linRect is null");
        }

        TextMeshProUGUI textComp = lineRect.GetComponentInChildren<TextMeshProUGUI>();
        bubble.GetComponentInChildren<TextMeshProUGUI>().text = msg;

        //  텍스트 길이에 맞춰 오른쪽 위치 조정
        float offsetX = textComp.preferredWidth + 30f;
        bubbleRect.anchoredPosition = new Vector2(offsetX, 0f);  // same Y as text
    }



    private bool CompareSyrups(List<SyrupRequirement> requiredSyrups)
    {
        // 총 시럽 개수 비교 (총 펌프 수)
        int expectedTotal = requiredSyrups.Sum(r => r.count);
        int actualTotal = syrupCounts.Sum(s => s.Value);
        if (expectedTotal != actualTotal) return false;

        // 각각의 시럽 이름과 펌프 횟수 비교
        foreach (var req in requiredSyrups)
        {
            if (!syrupCounts.ContainsKey(req.syrupName)) return false;
            if (syrupCounts[req.syrupName] != req.count) return false;
        }

        return true;
    }

    private string CalculateWhippedLevelFromGauge(float fillAmount)
    {
        if (whippedCreamGaugeImage == null) return "none";

        float fillImageWidth = whippedCreamGaugeImage.rectTransform.rect.width;
        float currentFillPosition = fillAmount * fillImageWidth;

        float lowArrowPos = GetArrowRelativePosition(whippedCreamGauageLowArrow, whippedCreamGaugeImage.rectTransform);
        float highArrowPos = GetArrowRelativePosition(whippedCreamGauageHighArrow, whippedCreamGaugeImage.rectTransform);
        float veryHighArrowPos = GetArrowRelativePosition(whippedCreamGauageveryHighArrow, whippedCreamGaugeImage.rectTransform);

        if (currentFillPosition >= veryHighArrowPos)
            return "veryhigh";
        else if (currentFillPosition >= highArrowPos)
            return "high";
        else if (currentFillPosition >= lowArrowPos)
            return "low";
        else if (fillAmount > 0)
            return "verylow";
        else
            return "none";
    }


    private string GetCurrentWhippedLevel()
    {
        return CalculateWhippedLevelFromGauge(whippedCreamGaugeImage.fillAmount);
    }


    private string GetWhippedCreamLevelName(float amount)
    {
        float width = whippedCreamGaugeImage.rectTransform.rect.width;
        float pos = amount * width;

        float low = GetArrowRelativePosition(whippedCreamGauageLowArrow, whippedCreamGaugeImage.rectTransform);
        float high = GetArrowRelativePosition(whippedCreamGauageHighArrow, whippedCreamGaugeImage.rectTransform);
        float veryHigh = GetArrowRelativePosition(whippedCreamGauageveryHighArrow, whippedCreamGaugeImage.rectTransform);

        if (pos >= veryHigh)
            return "veryhigh";
        else if (pos >= high)
            return "high";
        else if (pos >= low)
            return "low";
        else if (amount > 0)
            return "verylow";
        else
            return "none";
    }

    private bool SyrupCountsMatch(List<SyrupRequirement> expectedList)
    {
        // 비교할 시럽이 아예 없을 경우
        if ((expectedList == null || expectedList.Count == 0) && syrupCounts.Count == 0)
            return true;

        // 시럽 개수가 다르면 실패
        if (expectedList.Count != syrupCounts.Count)
            return false;

        foreach (var expected in expectedList)
        {
            if (!syrupCounts.TryGetValue(expected.syrupName, out int actualCount))
                return false;

            if (actualCount != expected.count)
                return false;
        }

        return true;
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
        if(currentPouredAmountText != null)
            currentPouredAmountText.text = $"{amount:F2} ml";
    }

    private void UpdateSyrupUI(int count, Transform targetTransform) // 횟수, targetTransform
    {
        if (syrupCountLabelPrefab == null) return;

        GameObject label = Instantiate(syrupCountLabelPrefab, syrupPumpingPanel.transform);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
        RectTransform parentRect = syrupPumpingPanel.GetComponent<RectTransform>();
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
