using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookingSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image dimOverlay; // ��ο� ��������

    private RecipeData recipe;
    private bool isCookable;

    public void SetRecipe(RecipeData data, bool cookable)
    {
        recipe = data;
        isCookable = cookable;

        iconImage.sprite = data.icon;
        dimOverlay.gameObject.SetActive(!cookable); // �帮�� ó��
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isCookable)
        {
            FindObjectOfType<CookingUIManager>().TryCook(recipe);
        }
    }
}
