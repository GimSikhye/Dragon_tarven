using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettlementSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dayText; // 일차
    [SerializeField] private TextMeshProUGUI totalIncomeText; // 전체 수익
    [SerializeField] private TextMeshProUGUI tipText; // 팁
    [SerializeField] private TextMeshProUGUI rentCostText; // 임대료
    [SerializeField] private TextMeshProUGUI refundText; // 환불
    [SerializeField] private TextMeshProUGUI netProfitText; // 순수익
    [SerializeField] private Button okButton;
    [SerializeField] private GameObject shopPanel;

    [Header("수치 설정")] // 수치도 게임 내에서 정해야 함
    public float totalIncome = 63.00f;
    public float tip = 14.26f;
    public float rentCost = 10.00f;
    public float refund = 0.00f;

    private float netProfit => totalIncome + tip - rentCost;

    void Start()
    {
        int day = PlayerPrefs.GetInt("Day", 1); // 기본 시작 1일차
        dayText.text = $"{day}일째";

        totalIncomeText.text = $"${totalIncome:F2}";
        tipText.text = $"${tip:F2}";
        rentCostText.text = $"-${rentCost:F2}";
        refundText.text = $"${refund:F2}";
        netProfitText.text = $"${netProfit:F2}";

        okButton.onClick.AddListener(OpenShop);
        PlayerStatsManager.Instance.AddCoin(netProfit); // 수익 반영
    }

    void OpenShop()
    {
        shopPanel.SetActive(true);
        okButton.gameObject.SetActive(false);
    }
}
