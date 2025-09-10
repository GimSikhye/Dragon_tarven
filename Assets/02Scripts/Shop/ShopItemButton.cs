using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    [Header("아이템 정보 UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;

    [Header("아이템 구매")]
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject checkMark;

    private ShopItemData shopItemData;
    private System.Action onPurchase;

    private string purchaseKey => $"Purchased_{shopItemData.itemName}"; // PlayerPrefs 키

    public void Init(ShopItemData data, System.Action onPurchaseCallback)
    {
        shopItemData = data;
        onPurchase = onPurchaseCallback;

        iconImage.sprite = data.icon;
        nameText.text = data.itemName;
        descriptionText.text = data.description;
        priceText.text = $"${data.price}";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuyItem);

        // 이미 구매했는지 확인
        bool alreadyPurchased = PlayerPrefs.GetInt(purchaseKey, 0) == 1;
        if (alreadyPurchased)
        {
            checkMark.SetActive(true);
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            checkMark.SetActive(false);
            buyButton.gameObject.SetActive(true);
        }
    }

    void BuyItem()
    {
        if (PlayerStatsManager.Instance.Coin < shopItemData.price)
        {
            Debug.Log("코인 부족");
            return;
        }

        PlayerStatsManager.Instance.AddCoin(-shopItemData.price);
        PlayerPrefs.SetInt("Coin", PlayerStatsManager.Instance.Coin);

        // 구매 상태 저장
        PlayerPrefs.SetInt(purchaseKey, 1);
        PlayerPrefs.Save();

        if (shopItemData.itemData != null)
            Inventory.Instance.AddItem(shopItemData.itemData, 1);

        checkMark.SetActive(true);
        buyButton.gameObject.SetActive(false);

        onPurchase?.Invoke();
    }
}
