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
        SetState(CustomerState.WalkingAround);
    }

    public void SetState(CustomerState newState)
    {
        switch(newState)
        {
            case CustomerState.WalkingAround:
                movement.WalkRandomly();
                break;

            case CustomerState.Entering:
                movement.MoveToCounter(() => SetState(CustomerState.Ordering));
                break;

            case CustomerState.Ordering:
                movement.PlayIdleAnimation();
                Invoke(nameof(GoToSeat), 2f); // 2�� �� �ڸ��� �̵�
                break;

            case CustomerState.MovingToSeat:
                movement.MoveToSeat(() => SetState(CustomerState.Moving));
                break;

            case CustomerState.Sitting:
                movement.Sit();
                break;

            case CustomerState.Leaving:
                movement.LeaveStore(() => Destroy(gameObject));
                break;
        }
    }

    private void GoToSeat()
    {
        SetState(CustomerState.MovingToSeat);
    }
}
