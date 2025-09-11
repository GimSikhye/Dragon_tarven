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
    [SerializeField] private TextMeshProUGUI netProfitAmountText; // 순수익

    [Header("캔버스 UI")]
    [SerializeField] private GameObject settlementCanvas;
    [SerializeField] private GameObject shopCanvas;

    [Header("버튼")]
    [SerializeField] private Button okayButton;

    [Header("수치 설정")]
    public int todayProfit = 0;   // 전체 수익
    public int rentCost = 10;    // 기본 임대료
    public int netProfit = 0;     // 순수익

    void Start()
    {
        settlementCanvas.SetActive(true);
        shopCanvas.SetActive(false);

        int day = PlayerPrefs.GetInt("Day", 1);
        currentDayAmountText.text = $"{day}일째";

        // 저장된 오늘 수익 불러오기
        todayProfit = PlayerPrefs.GetInt("TodayProfit", 0);
        todayProfitAmountText.text = $"${todayProfit}";

        // 임대료 표시
        rentCostAmountText.text = $"-${rentCost}";

        // 순수익 계산 (음수 허용)
        netProfit = todayProfit - rentCost;

        // 순수익 UI 표시 및 색상 처리
        netProfitAmountText.text = $"${netProfit}";
        netProfitAmountText.color = netProfit < 0 ? Color.red : Color.green;

        // PlayerStatsManager를 이용해서 코인 반영
        PlayerStatsManager.Instance.AddCoin(netProfit);

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
