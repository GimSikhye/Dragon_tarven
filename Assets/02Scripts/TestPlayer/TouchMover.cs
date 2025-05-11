using DalbitCafe.Map;
using DalbitCafe.Operations;
using DalbitCafe.UI;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(CharacterAnimatorController))]
// ��ġ �Է� ���� & �̵� ó��
public class TouchMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 3f;

    // �̵� �� UI ������ üũ
    private GraphicRaycaster uiRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    [Header("Ŀ�Ǹӽ� ����")]
    [SerializeField] private float _interactionRange = 0.3f; // ��ȣ�ۿ�

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

        // ���� ��ġ -> ��ǥ ��ġ�� �̵�
        if (targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                targetPosition = Vector3.zero; // �̵� ����
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
                    Debug.Log("UI ������ �� �� -> �̵� ����");
                    return;
                }

                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                worldPosition.z = 0f;

                //Ŀ�Ǹӽ� ��ǥ ������� ã��
                var machine = CoffeeMachineManager.Instance.GetMachineAtPosition(worldPosition); // GetMachingAtPosition ��ġ�� ���� �ӽ��� ������ �ӽ��� ������
                if (machine != null) // ��ġ�� ���� �ӽ��̶��
                {
                    TouchCoffeeMachine(machine);
                }


                if (!FloorManager.Instance.IsFloor(worldPosition)) return;

                    // ��ǥ ���� �� ���� ����
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

    private void TouchCoffeeMachine(CoffeeMachine machine) // ���� �˻�
    {
        if (Vector3.Distance(transform.position, machine.transform.position) <= _interactionRange)
        {
            CoffeeMachine.SetLastTouchedMachine(machine); // �ش� �ӽ��� ���������� ��ġ�� �ӽ����� ����

            if (machine.IsRoasting)
            {
                Debug.Log("Ŀ�ǰ� �ν��� ���Դϴ�");
                //UIManager.Instance.ShowCurrentMenuPopUp(); // ���� ����� �ִ� �޴� �˾�
                //GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu");
                //currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(machine);
            }
            else
            {
                Debug.Log("Ŀ�� ����� ������");
                //UIManager.Instance.ShowMakeCoffeePopUp();
            }
        }
        else
        {
            //UIManager.Instance.ShowCapitonText(); // �ʹ� �־��
            Debug.Log("�ʹ� �־��");
        }
    }
}
