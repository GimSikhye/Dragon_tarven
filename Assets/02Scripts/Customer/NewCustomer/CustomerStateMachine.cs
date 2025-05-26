using Unity.VisualScripting;
using UnityEngine;

public enum CustomerState
{
    WalkingAround, // �Ÿ� �ֺ� �ȱ�
    Entering, // ���Կ� ������ ��
    Ordering, // �ֹ� ��
    MovingToSeat, // �ڸ��� �̵� ��
    Sitting, // �ڸ��� �ɱ�
    Leaving // ���� ������ ������ ��
}
public class CustomerStateMachine : MonoBehaviour
{
    public CustomerState CurrentState { get; private set; }

    private CustomerMovement movement;

    public void Init()
    {
        movement = GetComponent<CustomerMovement>();
        SetState(CustomerState.WalkingAround); // �����ɶ� �ȱ�
    }

    public void SetState(CustomerState newState)
    {
        switch(newState)
        {
            case CustomerState.WalkingAround:
                //Debug.Log("���� �ȴ���");
                movement.WalkRandomly(); // ������ �ڸ��� ����?
                break;

            case CustomerState.Entering: // ���Կ� ��
                //Debug.Log("���Կ� ������");
                movement.MoveToCounter(() => SetState(CustomerState.Ordering)); // () => : SetState(CustomerState.Ordering)�� ȣ���ϴ� �͸��Լ�
                break;

            case CustomerState.Ordering:
                movement.PlayIdleAnimation();
                //Debug.Log("�ֹ� �� �ڸ� �̵�");
                Invoke(nameof(GoToSeat), 2f); // 2�� �� �ڸ��� �̵�
                break;

            case CustomerState.MovingToSeat:
                //Debug.Log("�ڸ��� ������");
                movement.MoveToSeat(() => SetState(CustomerState.Sitting));
                break;

            case CustomerState.Sitting:
                //Debug.Log("�ɾ��ִ���");
                movement.Sit();
                break;

            case CustomerState.Leaving:
                //Debug.Log("���Ը� ��������");
                movement.LeaveStore(() => Destroy(gameObject));
                break;
        }
    }

    private void GoToSeat()
    {
        SetState(CustomerState.MovingToSeat);
    }
}
