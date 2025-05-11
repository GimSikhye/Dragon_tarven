using DalbitCafe.Map;
using DalbitCafe.Operations;
using DalbitCafe.UI;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(CharacterAnimatorController))]
// 터치 입력 감지 & 이동 처리
public class TouchMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 3f;

    // 이동 시 UI 위인지 체크
    private GraphicRaycaster uiRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    [Header("커피머신 로직")]
    [SerializeField] private float _interactionRange = 0.3f; // 상호작용

    private CharacterAnimatorController animatorController;

    private void Start()
    {
        animatorController = GetComponent<CharacterAnimatorController>();

        uiRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    private void Update()
    {
        HandleTouch();

        // 현재 위치 -> 목표 위치로 이동
        if (targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                targetPosition = Vector3.zero; // 이동 종료
                animatorController.PlayIdle();
            }
        }




    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                if (IsTouchOverUI(touch.position))
                {
                    Debug.Log("UI 위에서 손 뗌 -> 이동 안함");
                    return;
                }

                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                worldPosition.z = 0f;

                //커피머신 좌표 기반으로 찾기
                var machine = CoffeeMachineManager.Instance.GetMachineAtPosition(worldPosition); // GetMachingAtPosition 터치한 곳에 머신이 있으면 머신을 가져옴
                if (machine != null) // 터치한 곳이 머신이라면
                {
                    TouchCoffeeMachine(machine);
                }


                if (!FloorManager.Instance.IsFloor(worldPosition)) return;

                    // 목표 갱신 및 방향 재계산
                    targetPosition = worldPosition;
                    animatorController.UpdateDirectionAndPlay(transform.position, targetPosition);

            }

        }
    }

    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    private void TouchCoffeeMachine(CoffeeMachine machine) // 여기 검사
    {
        if (Vector3.Distance(transform.position, machine.transform.position) <= _interactionRange)
        {
            CoffeeMachine.SetLastTouchedMachine(machine); // 해당 머신을 마지막으로 터치한 머신으로 설정

            if (machine.IsRoasting)
            {
                Debug.Log("커피가 로스팅 중입니다");
                //UIManager.Instance.ShowCurrentMenuPopUp(); // 현재 만들고 있는 메뉴 팝업
                //GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu");
                //currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(machine);
            }
            else
            {
                Debug.Log("커피 만들기 윈도우");
                //UIManager.Instance.ShowMakeCoffeePopUp();
            }
        }
        else
        {
            //UIManager.Instance.ShowCapitonText(); // 너무 멀어요
            Debug.Log("너무 멀어요");
        }
    }
}
