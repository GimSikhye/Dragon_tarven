using UnityEngine;
using UnityEngine.EventSystems;

public class Fridgeinteraction : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Fridge fridge;
    [SerializeField] private int slotCount = 18;

    private bool canTouch = true;
    private float touchCooldown;

    private void Awake()
    {
        fridge = GameObject.Find("Canvas_Fridge").GetComponent<Fridge>();
    }

    private void Update()
    {
        if (!canTouch)
        {
            touchCooldown -= Time.deltaTime;
            if (touchCooldown <= 0) canTouch = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canTouch) return;

        canTouch = false;
        touchCooldown = 1f;

        Debug.Log("³ÃÀå°í ÅÍÄ¡");
        fridge.ToggleFridgeUI(slotCount);
    }
}
