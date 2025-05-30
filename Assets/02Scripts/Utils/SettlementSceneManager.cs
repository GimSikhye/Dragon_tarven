using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettlementSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text totalIncomeText;
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private TMP_Text rentCostText;
    [SerializeField] private TMP_Text refundText;
    [SerializeField] private TMP_Text usedMaterialText;
    [SerializeField] private TMP_Text netProfitText;
    [SerializeField] private Button okButton;
    [SerializeField] private GameObject shopPanel;

    [Header("수치 설정")]
    public float totalIncome = 63.00f;
    public float tip = 14.26f;
    public float rentCost = 10.00f;
    public float refund = 0.00f;
    public float usedMaterial = 43.38f;

    private float netProfit => totalIncome + tip - rentCost - usedMaterial;

    void Start()
    {
        int day = PlayerPrefs.GetInt("Day", 1);
        dayText.text = $"{day}일째";

        totalIncomeText.text = $"${totalIncome:F2}";
        tipText.text = $"${tip:F2}";
        rentCostText.text = $"-${rentCost:F2}";
        refundText.text = $"${refund:F2}";
        usedMaterialText.text = $"-${usedMaterial:F2}";
        netProfitText.text = $"${netProfit:F2}";

        okButton.onClick.AddListener(OpenShop);
        PlayerStatsManager.Instance.AddCoin((int)netProfit); // 수익 반영
    }

    void OpenShop()
    {
        shopPanel.SetActive(true);
        okButton.gameObject.SetActive(false);
    }
}
