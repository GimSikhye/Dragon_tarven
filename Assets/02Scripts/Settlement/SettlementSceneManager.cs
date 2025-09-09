using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettlementSceneManager : MonoBehaviour
{
    [Header("영수증 UI")]
    [SerializeField] private TextMeshProUGUI currentDayAmountText; // 일차
    [SerializeField] private TextMeshProUGUI todayProfitAmountText; // 전체 수익
    [SerializeField] private TextMeshProUGUI rentCostAmountText; // 임대료

    [Header("캔버스 UI")]
    [SerializeField] private GameObject settlementCanvas;
    [SerializeField] private GameObject shopCanvas;

    [Header("버튼")]
    [SerializeField] private Button okayButton;

    [Header("수치 설정")] // 
    public int todayProfit = 0;

    void Start()
    {
        settlementCanvas.SetActive(true);
        shopCanvas.SetActive(false);

        int day = PlayerPrefs.GetInt("Day", 1);
        currentDayAmountText.text = $"{day}일째";

        // 저장된 오늘 수익 불러오기
        todayProfit = PlayerPrefs.GetInt("TodayProfit", 0);
        todayProfitAmountText.text = $"${todayProfit:F2}";

        okayButton.onClick.AddListener(OnConfirmButtonClicked);

    }

    void OpenShop()
    {
        shopCanvas.SetActive(true);
        settlementCanvas.SetActive(false);
    }

    public void OnConfirmButtonClicked()
    {        
        OpenShop();

    }
}
