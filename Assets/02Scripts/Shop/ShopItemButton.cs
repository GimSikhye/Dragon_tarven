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

    private ShopItemData shopItemData;
    private System.Action onPurchase; // �������� ��

    public void Init(ShopItemData data, System.Action onPurchaseCallback)
    {
        shopItemData = data;
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
        if (stats.Coin < shopItemData.price)
        {
            Debug.Log("���� ����");
            return;
        }

        stats.AddCoin(-shopItemData.price);

        // ��ġ�� �������̶��,
        if(shopItemData.itemData != null) 
        Inventory.Instance.AddItem(shopItemData.itemData, 1);

        checkMark.SetActive(true);
        buyButton.gameObject.SetActive(false);
        onPurchase?.Invoke();
    }

}
