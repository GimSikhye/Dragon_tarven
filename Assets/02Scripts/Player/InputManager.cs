using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector3> OnTouchEnded; // 터치 끝 이벤트 (월드 좌표 기준)

    [SerializeField] private GraphicRaycaster uiRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    void Awake()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                // 배치 모드일 때는 InputManager에서 터치 처리하지 않음
                if (DalbitCafe.Deco.DecorateManager.Instance != null &&
                    DalbitCafe.Deco.DecorateManager.Instance.IsDecorateMode)
                {
                    return;
                }

                if (IsTouchOverUI(touch.position))
                {
                    Debug.Log("UI 위에서 손 뗌 -> 이동 안함");
                    return;
                }

                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                worldPosition.z = 0f;
                OnTouchEnded?.Invoke(worldPosition);
            }
        }
    }

    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            Debug.Log("Raycast hit: " + result.gameObject.name);

            // 드래그 가능한 아이템이나 배치 관련 UI는 터치 차단하지 않음
            if (result.gameObject.CompareTag("DraggableItem") ||
                result.gameObject.name.Contains("Confirm") ||
                result.gameObject.name.Contains("Cancel") ||
                result.gameObject.name.Contains("Rotate"))
            {
                return false;
            }
        }

        return results.Count > 0;
    }
}