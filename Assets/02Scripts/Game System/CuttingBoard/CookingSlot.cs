using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookingSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;

    private RecipeData recipe;
    private bool isCookable;

    public void SetRecipe(RecipeData data, bool cookable)
    {
        recipe = data;
        isCookable = cookable;

        iconImage.sprite = data.icon;

        if (cookable)
        {
            iconImage.color = Color.white; // 원래 색
        }
        else
        {
            Color gray = Color.gray;
            gray.a = 0.75f; // 투명도 조정 (75%)
            iconImage.color = gray;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isCookable)
        {
            FindObjectOfType<CookingUIManager>().TryCook(recipe);
        }
    }
}
