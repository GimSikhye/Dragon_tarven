using DalbitCafe.Player;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using DalbitCafe.Operations;
using DalbitCafe.UI;
using System.Collections;
using DalbitCafe.Inputs;
public enum Windows
{
    MakeCoffee = 0,
    Exit = 1,
    CurrentMenu = 2,
    Quest = 3,
    QuestComplete = 4,
    Setting = 5
    // 필요한 만큼 아래에 계속 추가 가능
}

// ui 참조 다 날라간거 이어줘야 함
public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] public GameObject[] panels;
    [SerializeField] private TextMeshProUGUI _captionText; // 주의 문구
    [SerializeField] private Image _touchFeedback;


    [Header("재화량 텍스트")]
    [SerializeField] private TextMeshProUGUI _coffeeBeanAmountText;
    [SerializeField] private TextMeshProUGUI _coinAmountText;
    [SerializeField] private TextMeshProUGUI _gemAmountText;

    [Header("커피 머신 UI")]
    [SerializeField] private Slider _coffeeProgressSlider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    [SerializeField] private float _coffeeMakeDuration = 1f;
    private InventoryUI _inventoryUI;
    private StoreManager _storeManager;

    // 닷트윈 UI 애니메이션을 위한 재화 이전값
    private int _currentCoffeeBean;
    private int _currentCoin;
    private int _currentGem;

    // 프로필
    public Slider expSlider;
    public TextMeshProUGUI currentLevelText;

    public Action<QuestData> OnQuestComplete;

    private Coroutine _coffeeMakeCoroutine;

    private void StartCoffeeMaking(CoffeeData coffeeData)
    {
        if (_coffeeMakeCoroutine != null)
            StopCoroutine(_coffeeMakeCoroutine);

        _coffeeMakeCoroutine = StartCoroutine(CoffeeMakingRoutine(coffeeData));
    }

    
    private IEnumerator CoffeeMakingRoutine(CoffeeData coffeeData)
    {
        _coffeeProgressSlider.gameObject.SetActive(true);
        _sliderText.gameObject.SetActive(true);

        // 1. 커피머신의 월드 위치 -> 스크린 위치로 변환
        Vector3 worldPos = CoffeeMachine.LastTouchedMachine.transform.position + Vector3.up * 1.2f; // 약간 위로 띄움
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 2. UI 위치 적용
        _coffeeProgressSlider.transform.position = screenPos;
        _sliderText.transform.position = screenPos + new Vector3(0, 25f, 0); // 텍스트는 슬라이더 위로 조금

        _coffeeProgressSlider.value = 0;
        _sliderText.text = "커피가 만들어지는 중...";

        float elapsed = 0f;
        while (elapsed < _coffeeMakeDuration)
        {
            elapsed += Time.deltaTime;
            _coffeeProgressSlider.value = Mathf.Clamp01(elapsed / _coffeeMakeDuration);
            yield return null;
        }

        _sliderText.text = "커피 완성!";
        yield return new WaitForSeconds(0.5f);

        _coffeeProgressSlider.gameObject.SetActive(false);
        _sliderText.gameObject.SetActive(false);

        CoffeeMachine.LastTouchedMachine.RoastCoffee(coffeeData);
    }


    public void UpdateExpUI(int exp, int maxExp, int level)
    {
        if (expSlider != null)
        {
            expSlider.maxValue = maxExp;
            expSlider.value = exp; // 현재 경험치
        }

        if (currentLevelText != null)
        {
            currentLevelText.text = $"Lv {level}";
        }
    }

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += InitGameUI;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= InitGameUI;
    }

    private void OnEnable()
    {
       TouchInputManager.Instance.OnTouchBegan += ShowTouchFeedback;
       TouchInputManager.Instance.OnTouchMoved += ShowTouchFeedback;
    }

    private void OnDisable()
    {
        TouchInputManager.Instance.OnTouchBegan -= ShowTouchFeedback;
        TouchInputManager.Instance.OnTouchMoved -= ShowTouchFeedback;
    }


    private void InitGameUI(Scene scene, LoadSceneMode mode) // 게임씬 일때만 실행
    {
        if (scene.name != "GameScene") return;

        // GameScene의 UI 요소 다시 연결
        panels = GameObject.Find("UIPanels")?.GetComponentsInChildren<Transform>(true)
            ?.Where(t => t.CompareTag("UIPanel"))
            .Select(t => t.gameObject).ToArray();

        _captionText = GameObject.Find("UI_CaptionText")?.GetComponent<TextMeshProUGUI>();
        _touchFeedback = GameObject.Find("UI_TouchFeedback")?.GetComponent<Image>();
        _coffeeBeanAmountText = GameObject.Find("UI_CoffeeBeanAmountText")?.GetComponent<TextMeshProUGUI>();
        _coinAmountText = GameObject.Find("UI_CoinAmountText")?.GetComponent<TextMeshProUGUI>();
        _gemAmountText = GameObject.Find("UI_GemAmountText")?.GetComponent<TextMeshProUGUI>();

        expSlider = GameObject.Find("UI_expbar")?.GetComponent<Slider>();
        currentLevelText = GameObject.Find("UI_LevelText")?.GetComponent<TextMeshProUGUI>();

        _coffeeProgressSlider = GameObject.Find("Canvas_GameScene").transform.Find("UI_CoffeeProgressSlider")?.GetComponent<Slider>();
        _sliderText = _coffeeProgressSlider.transform.Find("UI_CoffeeProgressText")?.GetComponent<TextMeshProUGUI>();
        Debug.Log(_coffeeProgressSlider != null);
        _sliderText.gameObject.SetActive(false);
        _coffeeProgressSlider.gameObject.SetActive(false);
        _inventoryUI = GameObject.Find("InventoryUI").GetComponent<InventoryUI>();
        _storeManager = GameObject.Find("StoreManager").GetComponent<StoreManager>();

        // 패널 숨기기 + 초기화
        if (panels != null)
        {
            foreach (var panel in panels)
                panel.SetActive(false);
        }
        InitializeAllButtons();


        // 데이터 바인딩
        var stats = PlayerStatsManager.Instance;
        UpdateCoffeeBeanUI(stats.CoffeeBeans);
        UpdateCoinUI(stats.Coin);
        UpdateGemUI(stats.Gem);
        UpdateExpUI(stats.Exp, stats.MaxExp, stats.Level);
    }


    public void UpdateCoffeeBeanUI(int value)
    {
        TextAnimationHelper.AnimateNumber(_coffeeBeanAmountText, _currentCoffeeBean, value);
        _currentCoffeeBean = value;
    }

    public void UpdateCoinUI(int value)
    {
        TextAnimationHelper.AnimateNumber(_coinAmountText, _currentCoin, value, 1.5f);
        _currentCoin = value;
    }

    public void UpdateGemUI(int value)
    {
        TextAnimationHelper.AnimateNumber(_gemAmountText, _currentGem, value); // to int
        _currentGem = value;
    }

    public void ShowMakeCoffeePopUp()
    {
        var panel = panels[(int)Windows.MakeCoffee]; // 번호가 제대로 할당안됨
        panel.SetActive(true);
        // 애니메이션
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void ShowExitPopUp()
    {
        panels[(int)Windows.Exit].SetActive(true);
    }

    public void ShowCapitonText()
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(FindObjectOfType<PlayerCtrl>().transform.position);
        _captionText.rectTransform.position = playerScreenPos;
        _captionText.enabled = true;
        _captionText.text = "거리가 너무 멀어요!";
    }

    public void ShowCurrentMenuPopUp()
    {
        GameObject window = panels[(int)Windows.CurrentMenu];
        window.SetActive(true);
        window.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        window.GetComponent<Image>().DOFade(1, 0.5f);
    }

    public void ShowQuestPopUp()
    {
        GameObject window = panels[(int)Windows.Quest];
        window.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => window.SetActive(true));
    }

    public void ShowExitPopUp(string window)
    {
        GameObject windowPanel = GameObject.Find(window);
        windowPanel.SetActive(true);
    }

    public bool IsTouchOverUIPosition(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    private void ShowTouchFeedback(Vector2 screenPosition)
    {
        if(_touchFeedback != null)
        {
            _touchFeedback.rectTransform.position = screenPosition;
            _touchFeedback.enabled = true;
        }

    }

    public void OpenInventory()
    {
        _inventoryUI.Open();
    }

    public void CloseInventory()
    {
        _inventoryUI.Close();
    }

    public void OpenStore()
    {
        _storeManager.Open();   
    }

    private void InitializeAllButtons()
    {
        Debug.Log("InitializeAllButtons 호출"); // 여기에 로그 추가

        foreach (var panel in panels)
        {
            Debug.Log(panel.name);
            var buttons = panel.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                string btnName = btn.name;
                btn.onClick.RemoveAllListeners();

                btn.onClick.AddListener(() =>
                {
                    //GameManager.Instance.SoundManager.PlaySFX(click_clip); // 클릭 효과음
                    Debug.Log($"{btnName} 버튼 클릭!");
                    // 버튼 이름으로 처리 분기
                    switch (btnName)
                    {
                        case "make_btn":
                            {
                                Transform container = btn.transform.parent;
                                RoastingWindow roastingWindow = panel.GetComponent<RoastingWindow>();
                                if (roastingWindow == null) return;

                                int index = roastingWindow.menuContainers.IndexOf(container.gameObject);
                                if (index >= 0 && index < roastingWindow.coffeDataList.Count)
                                {
                                    CoffeeData dataToRoast = roastingWindow.coffeDataList[index];
                                    Debug.Log($"커피 만들기: {dataToRoast.CoffeeName}");

                                    // 기존 커피 굽기 로직 주석 처리하고 슬라이더 연출 시작
                                    // CoffeeMachine.LastTouchedMachine.RoastCoffee(dataToRoast);
                                    StartCoffeeMaking(dataToRoast);
                                }
                                else
                                {
                                    Debug.LogWarning("Menu container를 찾을 수 없거나 index가 유효하지 않음");
                                }
                                break;
                            }
                        case "close_btn":
                            panel.SetActive(false);
                            break;
                        // 추가 케이스...
                        case "UI_closeBtn":
                            panel.SetActive(false);
                            break;
                        case "UI_CompleteButton":
                            Debug.Log("완료");
                            break;
                    }
                });
            }
        }
    }


}
