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
public class MovementHandler : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 3f;

    private CharacterAnimatorController animatorController;

    private void Start()
    {
        animatorController = GetComponent<CharacterAnimatorController>();
        InputManager.OnTouchEnded += HandleTouch;

    }

    private void OnDestroy()
    {
        InputManager.OnTouchEnded -= HandleTouch;

    }

    private void Update()
    {
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

    private void HandleTouch(Vector3 worldPos)
    {
        if (!FloorManager.Instance.IsFloor(worldPos)) return;

        targetPosition = worldPos;
        animatorController.UpdateDirectionAndPlay(transform.position, targetPosition);
    }

}

