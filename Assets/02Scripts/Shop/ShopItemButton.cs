using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject checkMark;

    private ShopItemData itemData;
    private System.Action onPurchase;

    public void Init(ShopItemData data, System.Action onPurchaseCallback)
    {
        itemData = data;
        onPurchase = onPurchaseCallback;

        iconImage.sprite = data.icon;
        nameText.text = data.itemName;
        descText.text = data.description;
        priceText.text = $"${data.price}";

        buyButton.onClick.AddListener(BuyItem);
        checkMark.SetActive(false);
    }

    void BuyItem()
    {
        var stats = PlayerStatsManager.Instance;
        if (stats.Coin < itemData.price)
        {
            Debug.Log("코인 부족");
            return;
        }

        stats.AddCoin(-itemData.price);

        // 핵심 수정: ShopItemData → 내부의 itemData 사용
        Inventory.Instance.AddItem(itemData.itemData, 1);

        checkMark.SetActive(true);
        buyButton.gameObject.SetActive(false);
        onPurchase?.Invoke();
    }

}
