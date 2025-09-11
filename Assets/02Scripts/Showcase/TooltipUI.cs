using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] public GameObject tooltipObject;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    

    public void SetData(string itemName, int price)
    {
        nameText.text = itemName;
        priceText.text = $"가격: {price} 코인";
    }
}
