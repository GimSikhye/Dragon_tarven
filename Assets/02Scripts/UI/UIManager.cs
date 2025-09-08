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

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject[] panels;
    [SerializeField] private TextMeshProUGUI _captionText; // 주의 문구


    [Header("재화량 텍스트")]
    [SerializeField] private TextMeshProUGUI _coffeeBeanAmountText;
    [SerializeField] private TextMeshProUGUI _coinAmountText;
    [SerializeField] private TextMeshProUGUI _gemAmountText;

    [Header("커피 머신 UI")]
    [SerializeField] CoffeeMachineManager coffeeMachineManager;
    [SerializeField] private Slider _coffeeProgressSlider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    [SerializeField] private float _coffeeMakeDuration = 1f;

    [Header("버튼들")]
    [SerializeField] Button questButton;
    [SerializeField] Button menuButton;

    private InventoryUI _inventoryUI;
    private StoreManager _storeManager;

    // 닷트윈 UI 애니메이션을 위한 재화 이전값
    private int _currentCoffeeBean;
    private float _currentCoin;
    private int _currentGem;

    public Action<QuestData> OnQuestComplete;

    private Coroutine _coffeeMakeCoroutine;
    private void OnEnable()
    {
        PlayerStatsManager.OnCoinChanged += UpdateCoinUI;
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnCoinChanged -= UpdateCoinUI;
    }
    private void Start()
    {
        questButton.onClick.AddListener(() => ShowQuestPopUp());
        //menuButton.onClick.AddListener(() => ShowQuestPopUp);
    }
    private void StartCoffeeMaking(CoffeeData coffeeData)
    {
        Debug.Log("커피 만들기 시작");
        if (_coffeeMakeCoroutine != null)
            StopCoroutine(_coffeeMakeCoroutine);

        _coffeeMakeCoroutine = StartCoroutine(CoffeeMakingRoutine(coffeeData));
    }
    
    private IEnumerator CoffeeMakingRoutine(CoffeeData coffeeData)
    {
        _coffeeProgressSlider.gameObject.SetActive(true);
        _sliderText.gameObject.SetActive(true);

        // 커피머신의 월드 위치 -> 스크린 위치로 변환
        Vector3 worldPos = coffeeMachineManager.transform.position + Vector3.up * 1.2f; // 약간 위로 띄움
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

        coffeeMachineManager.lastTouchedMachine.RoastCoffee(coffeeData); // 로스트커피
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
        var panel = panels[(int)Windows.MakeCoffee];
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
        //Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(FindObjectOfType<PlayerCtrl>().transform.position);
        //_captionText.rectTransform.position = playerScreenPos;
        _captionText.enabled = true;
        _captionText.text = "거리가 너무 멀어요!";
    }

    public void ShowCurrentMenuPopUp()
    {
        GameObject window = panels[(int)Windows.CurrentMenu];
        window.SetActive(true);
        window.transform.Find("menu icon").GetComponent<Image>().sprite = coffeeMachineManager.lastTouchedMachine.CurrentCoffee.MenuIcon;
        window.transform.Find("menu name").GetComponent<TextMeshProUGUI>().text = coffeeMachineManager.lastTouchedMachine.CurrentCoffee.CoffeeName;
        window.transform.Find("remainingMugsText").GetComponent<TextMeshProUGUI>().text = $"{coffeeMachineManager.lastTouchedMachine.RemainingMugs.ToString()}잔 남음";
        //window.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        //window.GetComponent<Image>().DOFade(1, 0.5f);
    }

    public void ShowQuestPopUp()
    {
        Debug.Log("퀘스트 창 열림");
        GameObject questWindow = panels[(int)Windows.Quest];
        questWindow.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => questWindow.SetActive(true));
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

    private void Awake()
    {
        InitializeAllButtons();
    }
    private void InitializeAllButtons()
    {
        foreach (var panel in panels)
        {
            Button[] buttons = panel.GetComponentsInChildren<Button>(true); ; // 패널의 버튼 자식들을 모두 가져옴
            foreach (var button in buttons)
            {
                string buttonName = button.name;
                button.onClick.RemoveAllListeners(); // 모든 클릭 이벤트 제거

                button.onClick.AddListener(() =>
                {
                    Debug.Log($"{buttonName} 버튼 클릭!");

                    // 버튼 이름으로 처리 분기
                    switch (buttonName)
                    {
                        case "menuMakeButton":
                            {
                                Transform menuContainer = button.transform.parent; // 해당 버튼의 menuContainer 할당
                                RoastingWindow roastingWindow = panel.GetComponent<RoastingWindow>();
                                if (roastingWindow == null) return;

                                int index = roastingWindow.coffeeMachineMenuContainers.IndexOf(menuContainer.gameObject); // 해당 menuContainer가 몇 번째 인덱스에 있는지 할당
                                if (index >= 0 && index < roastingWindow.coffeDataList.Count) // Index가 0보다 크고, coffeDataSO List 요소 개수보다 작아야 함.
                                {
                                    CoffeeData menuDataToRoast = roastingWindow.coffeDataList[index]; // coffeDataList Index와 roastringWindow coffeeMachineMenuContainers와 인덱스 맞춰야 함.
                                    Debug.Log($"커피 만들기: {menuDataToRoast.CoffeeName}");

                                    StartCoffeeMaking(menuDataToRoast);
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
