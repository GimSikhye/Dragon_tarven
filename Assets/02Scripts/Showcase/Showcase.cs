using UnityEngine;
using UnityEngine.EventSystems;

public class Showcase : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ShowcaseManager showcaseManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        showcaseManager.ToggleShowcaseUI();
    }

}
