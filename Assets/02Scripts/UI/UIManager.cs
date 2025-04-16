using DalbitCafe.Player;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
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


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TextMeshProUGUI _captionText; // ���� ����

    [Header("��ȭ�� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI _coffeeBeanAmountText;
    [SerializeField] private TextMeshProUGUI _coinAmountText;
    [SerializeField] private TextMeshProUGUI gemAmountText;

    // ��Ʈ�� UI �ִϸ��̼��� ���� ��ȭ ������
    private int _currentCoffeeBean;
    private int _currentCoin;
    private int _currentGem;

    public Slider expSlider;
    public TextMeshProUGUI currentLevelText;

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

    void Awake()
    {
        SceneManager.sceneLoaded += InitGameUI;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= InitGameUI;
    }


    private void InitGameUI(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene") return;

        // GameScene�� UI ��� �ٽ� ����
        _panels = GameObject.Find("UIPanels")?.GetComponentsInChildren<Transform>(true)
            ?.Where(t => t.CompareTag("UIPanel"))  
            .Select(t => t.gameObject).ToArray(); 

        _captionText = GameObject.Find("UI_CaptionText")?.GetComponent<TextMeshProUGUI>();

        _coffeeBeanAmountText = GameObject.Find("UI_CoffeeBeanAmountText")?.GetComponent<TextMeshProUGUI>();
        _coinAmountText = GameObject.Find("UI_CoinAmountText")?.GetComponent<TextMeshProUGUI>();
        gemAmountText = GameObject.Find("UI_GemAmountText")?.GetComponent<TextMeshProUGUI>();

        expSlider = GameObject.Find("UI_ExpSlider")?.GetComponent<Slider>();
        currentLevelText = GameObject.Find("UI_LevelText")?.GetComponent<TextMeshProUGUI>();

        // �г� ����� + �ʱ�ȭ
        if (_panels != null)
        {
            foreach (var panel in _panels)
                panel.SetActive(false);
        }

        // ������ ���ε�
        var stats = GameManager.Instance.PlayerStatsManager;
        UpdateCoffeeBeanUI(stats.CoffeeBeans);
        UpdateCoinUI(stats.Coin);
        UpdateGemUI(stats.Gem);
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
        TextAnimationHelper.AnimateNumber(gemAmountText, _currentGem, value); // to int
        _currentGem = value;
    }

    public void ShowMakeCoffeePopUp()
    {
        var panel = _panels[(int)Windows.MakeCoffee];
        panel.SetActive(true);
        // �ִϸ��̼�
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
        _captionText.text = "�Ÿ��� �ʹ� �־��!";
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

    public bool IsTouchOverUI(Touch touch)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }


}
