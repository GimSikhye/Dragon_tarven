using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SellConfirmUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Action onConfirm;

    private void Awake()
    {
        panel.SetActive(false);
        yesButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            panel.SetActive(false);
        });
        noButton.onClick.AddListener(() => panel.SetActive(false));
    }

    public void Show(string itemName, Action confirmAction)
    {
        messageText.text = $"{itemName}을(를) 판매하시겠습니까?";
        onConfirm = confirmAction;
        panel.SetActive(true);
    }
}
