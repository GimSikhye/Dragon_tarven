using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettlementSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dayText; // ����
    [SerializeField] private TextMeshProUGUI totalIncomeText; // ��ü ����
    [SerializeField] private TextMeshProUGUI tipText; // ��
    [SerializeField] private TextMeshProUGUI rentCostText; // �Ӵ��
    [SerializeField] private TextMeshProUGUI refundText; // ȯ��
    [SerializeField] private TextMeshProUGUI netProfitText; // ������
    [SerializeField] private Button okButton;
    [SerializeField] private GameObject shopPanel;

    [Header("��ġ ����")] // ��ġ�� ���� ������ ���ؾ� ��
    public float totalIncome = 63.00f;
    public float tip = 14.26f;
    public float rentCost = 10.00f;
    public float refund = 0.00f;

    private float netProfit => totalIncome + tip - rentCost;

    void Start()
    {
        int day = PlayerPrefs.GetInt("Day", 1); // �⺻ ���� 1����
        dayText.text = $"{day}��°";

        totalIncomeText.text = $"${totalIncome:F2}";
        tipText.text = $"${tip:F2}";
        rentCostText.text = $"-${rentCost:F2}";
        refundText.text = $"${refund:F2}";
        netProfitText.text = $"${netProfit:F2}";

        okButton.onClick.AddListener(OpenShop);
        PlayerStatsManager.Instance.AddCoin(netProfit); // ���� �ݿ�
    }

    void OpenShop()
    {
        shopPanel.SetActive(true);
        okButton.gameObject.SetActive(false);
    }
}
