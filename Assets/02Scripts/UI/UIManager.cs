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
    // �ʿ��� ��ŭ �Ʒ��� ��� �߰� ����
}

// ui ���� �� ���󰣰� �̾���� ��
public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] public GameObject[] panels;
    [SerializeField] private TextMeshProUGUI _captionText; // ���� ����
    [SerializeField] private Image _touchFeedback;


    [Header("��ȭ�� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI _coffeeBeanAmountText;
    [SerializeField] private TextMeshProUGUI _coinAmountText;
    [SerializeField] private TextMeshProUGUI _gemAmountText;

    [Header("Ŀ�� �ӽ� UI")]
    [SerializeField] private Slider _coffeeProgressSlider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    [SerializeField] private float _coffeeMakeDuration = 1f;
    private InventoryUI _inventoryUI;
    private StoreManager _storeManager;

    // ��Ʈ�� UI �ִϸ��̼��� ���� ��ȭ ������
    private int _currentCoffeeBean;
    private int _currentCoin;
    private int _currentGem;

    // ������
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

        // 1. Ŀ�Ǹӽ��� ���� ��ġ -> ��ũ�� ��ġ�� ��ȯ
        Vector3 worldPos = CoffeeMachine.LastTouchedMachine.transform.position + Vector3.up * 1.2f; // �ణ ���� ���
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 2. UI ��ġ ����
        _coffeeProgressSlider.transform.position = screenPos;
        _sliderText.transform.position = screenPos + new Vector3(0, 25f, 0); // �ؽ�Ʈ�� �����̴� ���� ����

        _coffeeProgressSlider.value = 0;
        _sliderText.text = "Ŀ�ǰ� ��������� ��...";

        float elapsed = 0f;
        while (elapsed < _coffeeMakeDuration)
        {
            elapsed += Time.deltaTime;
            _coffeeProgressSlider.value = Mathf.Clamp01(elapsed / _coffeeMakeDuration);
            yield return null;
        }

        _sliderText.text = "Ŀ�� �ϼ�!";
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
            expSlider.value = exp; // ���� ����ġ
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


    private void InitGameUI(Scene scene, LoadSceneMode mode) // ���Ӿ� �϶��� ����
    {
        if (scene.name != "GameScene") return;

        // GameScene�� UI ��� �ٽ� ����
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

        // �г� ����� + �ʱ�ȭ
        if (panels != null)
        {
            foreach (var panel in panels)
                panel.SetActive(false);
        }
        InitializeAllButtons();


        // ������ ���ε�
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
        var panel = panels[(int)Windows.MakeCoffee]; // ��ȣ�� ����� �Ҵ�ȵ�
        panel.SetActive(true);
        // �ִϸ��̼�
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
        _captionText.text = "�Ÿ��� �ʹ� �־��!";
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
        Debug.Log("InitializeAllButtons ȣ��"); // ���⿡ �α� �߰�

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
                    //GameManager.Instance.SoundManager.PlaySFX(click_clip); // Ŭ�� ȿ����
                    Debug.Log($"{btnName} ��ư Ŭ��!");
                    // ��ư �̸����� ó�� �б�
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
                                    Debug.Log($"Ŀ�� �����: {dataToRoast.CoffeeName}");

                                    // ���� Ŀ�� ���� ���� �ּ� ó���ϰ� �����̴� ���� ����
                                    // CoffeeMachine.LastTouchedMachine.RoastCoffee(dataToRoast);
                                    StartCoffeeMaking(dataToRoast);
                                }
                                else
                                {
                                    Debug.LogWarning("Menu container�� ã�� �� ���ų� index�� ��ȿ���� ����");
                                }
                                break;
                            }
                        case "close_btn":
                            panel.SetActive(false);
                            break;
                        // �߰� ���̽�...
                        case "UI_closeBtn":
                            panel.SetActive(false);
                            break;
                        case "UI_CompleteButton":
                            Debug.Log("�Ϸ�");
                            break;
                    }
                });
            }
        }
    }


}
