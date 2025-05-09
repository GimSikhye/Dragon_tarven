using UnityEngine;


[RequireComponent(typeof(CharacterAnimatorController))]
// ��ġ �Է� ���� & �̵� ó��
public class TouchMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 3f;

    private CharacterAnimatorController animatorController;

    private void Start()
    {
        animatorController = GetComponent<CharacterAnimatorController>();
    }

    private void Update()
    {
        HandleTouch();

        // ���� ��ġ -> ��ǥ ��ġ�� �̵�
        if(targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, targetPosition) < 0.05f)
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
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                worldPos.z = 0f;

                // ��ǥ ���� �� ���� ����
                targetPosition = worldPos;
                animatorController.UpdateDirectionAndPlay(transform.position, targetPosition);


            }

        }
    }
}
