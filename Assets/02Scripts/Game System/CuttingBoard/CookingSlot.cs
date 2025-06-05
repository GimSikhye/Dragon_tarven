using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookingSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image dimOverlay; // 어두운 오버레이

    private RecipeData recipe;
    private bool isCookable;

    public void SetRecipe(RecipeData data, bool cookable)
    {
        recipe = data;
        isCookable = cookable;

        iconImage.sprite = data.icon;
        dimOverlay.gameObject.SetActive(!cookable); // 흐리게 처리
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isCookable)
        {
            FindObjectOfType<CookingUIManager>().TryCook(recipe);
        }
    }
}
