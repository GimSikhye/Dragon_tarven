using DalbitCafe.Player;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TextMeshProUGUI _captionText;

    [Header("재화량 텍스트")]
    [SerializeField] private TextMeshProUGUI _coffeeBeanText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI gemText;

    private int _currentCoffeeBean;
    private int _currentCoin;
    private int _currentGem;

    public Slider expSlider;
    public TextMeshProUGUI levelText;

    public void UpdateExpUI(int exp, int maxExp, int level)
    {
        if (expSlider != null)
        {
            expSlider.maxValue = maxExp;
            expSlider.value = exp;
        }

        if (levelText != null)
        {
            levelText.text = $"Lv {level}";
        }
    }

    private void Start()
    {
        foreach (var panel in _panels)
            panel.SetActive(false);

        var stats = GameManager.Instance.PlayerStatsManager;
        UpdateCoffeeBeanUI(stats.CoffeeBeans);
        UpdateCoinUI(stats.Coin);
        UpdateGemUI(stats.Gem);
    }

    public void UpdateCoffeeBeanUI(int value)
    {
        TextAnimationHelper.AnimateNumber(_coffeeBeanText, _currentCoffeeBean, value);
        _currentCoffeeBean = value;
    }

    public void UpdateCoinUI(int value)
    {
        TextAnimationHelper.AnimateNumber(_coinText, _currentCoin, value, 1.5f);
        _currentCoin = value;
    }

    public void UpdateGemUI(int value)
    {
        TextAnimationHelper.AnimateNumber(gemText, _currentGem, value);
        _currentGem = value;
    }

    public void ShowMakeCoffeePopUp()
    {
        var panel = _panels[(int)Windows.MakeCoffee];
        panel.SetActive(true);
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

    public enum Windows
    {
        MakeCoffee = 0,
        Exit = 1,
        CurrentMenu = 2,
        Quest = 3,
        // 필요한 만큼 아래에 계속 추가 가능
    }

}
