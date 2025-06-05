using UnityEngine;
using UnityEngine.UI;

public class FridgeSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void SetItem(Sprite icon)
    {
        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
    }
}
