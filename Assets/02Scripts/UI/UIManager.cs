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
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TextMeshProUGUI _captionText; // 주의 문구
    [SerializeField] private Image _touchFeedback;


    [Header("재화량 텍스트")]
    [SerializeField] private TextMeshProUGUI _coffeeBeanAmountText;
    [SerializeField] private TextMeshProUGUI _coinAmountText;
    [SerializeField] private TextMeshProUGUI _gemAmountText;

    // 닷트윈 UI 애니메이션을 위한 재화 이전값
    private int _currentCoffeeBean;
    private int _currentCoin;
    private int _currentGem;

    // 프로필
    public Slider expSlider;
    public TextMeshProUGUI currentLevelText;

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

    void Awake()
    {
        SceneManager.sceneLoaded += InitGameUI;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= InitGameUI;
    }

    private void OnEnable()
    {
        GameManager.Instance.TouchInputManager.OnTouchBegan += ShowTouchFeedback;
        GameManager.Instance.TouchInputManager.OnTouchMoved += ShowTouchFeedback;
    }

    private void OnDisable()
    {
        GameManager.Instance.TouchInputManager.OnTouchBegan -= ShowTouchFeedback;
        GameManager.Instance.TouchInputManager.OnTouchMoved -= ShowTouchFeedback;
    }


    private void InitGameUI(Scene scene, LoadSceneMode mode) // 게임씬 일때만 실행
    {
        if (scene.name != "GameScene") return;

        // GameScene의 UI 요소 다시 연결
        _panels = GameObject.Find("UIPanels")?.GetComponentsInChildren<Transform>(true)
            ?.Where(t => t.CompareTag("UIPanel"))
            .Select(t => t.gameObject).ToArray();

        _captionText = GameObject.Find("UI_CaptionText")?.GetComponent<TextMeshProUGUI>();
        _touchFeedback = GameObject.Find("UI_TouchFeedback")?.GetComponent<Image>();
        _coffeeBeanAmountText = GameObject.Find("UI_CoffeeBeanAmountText")?.GetComponent<TextMeshProUGUI>();
        _coinAmountText = GameObject.Find("UI_CoinAmountText")?.GetComponent<TextMeshProUGUI>();
        _gemAmountText = GameObject.Find("UI_GemAmountText")?.GetComponent<TextMeshProUGUI>();

        expSlider = GameObject.Find("UI_ExpSlider")?.GetComponent<Slider>();
        currentLevelText = GameObject.Find("UI_LevelText")?.GetComponent<TextMeshProUGUI>();

        // 패널 숨기기 + 초기화
        if (_panels != null)
        {
            foreach (var panel in _panels)
                panel.SetActive(false);
        }
        InitializeAllButtons();


        // 데이터 바인딩
        var stats = GameManager.Instance.PlayerStatsManager;
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
        var panel = _panels[(int)Windows.MakeCoffee]; // 번호가 제대로 할당안됨
        panel.SetActive(true);
        // 애니메이션
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void ShowExitPopUp()
    {
        _panels[(int)Windows.Exit].SetActive(true);
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
        GameObject window = _panels[(int)Windows.CurrentMenu];
        window.SetActive(true);
        window.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        window.GetComponent<Image>().DOFade(1, 0.5f);
    }

    public void ShowQuestPopUp()
    {
        GameObject window = _panels[(int)Windows.Quest];
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
        _touchFeedback.rectTransform.position = screenPosition;
        _touchFeedback.enabled = true;
    }

    private void InitializeAllButtons()
    {
        Debug.Log("InitializeAllButtons 호출"); // 여기에 로그 추가

        foreach (var panel in _panels)
        {
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

                            // Panel_MenuContainer 오브젝트를 찾음 (버튼의 부모의 부모일 수 있음)
                            Transform container = btn.transform.parent;

                            RoastingWindow roastingWindow = panel.GetComponent<RoastingWindow>();
                            if (roastingWindow == null) return;

                            // 해당 container가 menuContainers 중 몇 번째인지 확인
                            int index = roastingWindow.menuContainers.IndexOf(container.gameObject);

                            if (index >= 0 && index < roastingWindow.coffeDataList.Count)
                            {
                                CoffeeData dataToRoast = roastingWindow.coffeDataList[index];
                                Debug.Log($"커피 만들기: {dataToRoast.CoffeeName}");
                                CoffeeMachine.LastTouchedMachine.RoastCoffee(dataToRoast);
                            }
                            else
                            {
                                Debug.LogWarning("Menu container를 찾을 수 없거나 index가 유효하지 않음");
                            }
                            break;
                        case "close_btn":
                            panel.SetActive(false);
                            break;
                        // 추가 케이스...
                        case "UI_closeBtn":
                            panel.SetActive(false);
                            break;
                    }
                });
            }
        }
    }


}
