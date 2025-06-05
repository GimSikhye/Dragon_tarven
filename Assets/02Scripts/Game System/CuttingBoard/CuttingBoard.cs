using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingBoard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private CookingUIManager cookingUIManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        cookingUIManager.ToggleCookingUI();
    }

}
