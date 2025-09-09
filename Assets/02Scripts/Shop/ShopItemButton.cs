using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopItemButton : MonoBehaviour
{
    [Header("아이템 정보 UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText; // 아이템 설명
    [SerializeField] private TMP_Text priceText; // 아이템 가격

    [Header("아이템 구매")]
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject checkMark;

    private ShopItemData shopItemData;
    private System.Action onPurchase; // 구입했을 때 이벤트

    public void Init(ShopItemData data, System.Action onPurchaseCallback)
    {
        shopItemData = data;
        Debug.Log("아이템 버튼 Init");
        onPurchase = onPurchaseCallback;

        iconImage.sprite = data.icon; 
        nameText.text = data.itemName;
        descriptionText.text = data.description;
        priceText.text = $"${data.price}";

        buyButton.onClick.AddListener(BuyItem);
        checkMark.SetActive(false);
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
        PlayerPrefs.Save();

        // 배치용 아이템이라면,
        if (shopItemData.itemData != null) 
        Inventory.Instance.AddItem(shopItemData.itemData, 1);

        checkMark.SetActive(true);
        buyButton.gameObject.SetActive(false); // 버튼 비활성화
        onPurchase?.Invoke(); // 코인 UI 갱신
    }

}
