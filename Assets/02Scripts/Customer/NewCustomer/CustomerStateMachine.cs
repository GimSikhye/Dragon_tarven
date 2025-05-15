using Unity.VisualScripting;
using UnityEngine;

public enum CustomerState
{
    WalkingAround, // 거리 주변 걷기
    Entering, // 가게에 들어오는 중
    Ordering, // 주문 중
    MovingToSeat, // 자리에 이동 중
    Sitting, // 자리에 앉기
    Leaving // 가게 밖으로 떠나는 중
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
                Invoke(nameof(GoToSeat), 2f); // 2초 후 자리로 이동
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
