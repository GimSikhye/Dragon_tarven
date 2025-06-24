using Unity.VisualScripting;
using UnityEngine;

public enum CustomerState
{
    WalkingAround, // 거리 주변 걷기
    Entering,      // 가게에 들어오는 중
    Ordering,      // 주문 중
    MovingToSeat,  // 자리에 이동 중
    Sitting,       // 자리에 앉기
    Leaving        // 가게 밖으로 떠나는 중
}

public class CustomerStateMachine : MonoBehaviour
{
    public CustomerState CurrentState { get; private set; }

    private CustomerMovement movement;
    private Animator animator;

    public void Init()
    {
        movement = GetComponent<CustomerMovement>();
        animator = GetComponent<Animator>();

        //Debug.Log("[CustomerStateMachine] Init 호출됨 - 상태: WalkingAround");
        SetState(CustomerState.WalkingAround);
    }

    public void SetState(CustomerState newState)
    {
        CurrentState = newState; // 현재 상태 업데이트

        // 상태 전환시 IsSitting 파라미터 자동 관리
        if (newState == CustomerState.Sitting)
        {
            animator.SetBool("IsSitting", true);
        }
        else
        {
            animator.SetBool("IsSitting", false);
        }

        switch (newState)
        {
            case CustomerState.WalkingAround:
                movement.WalkRandomly();
                break;

            case CustomerState.Entering:
                movement.MoveToCounter(() => SetState(CustomerState.Ordering));
                break;

            case CustomerState.Ordering:
                movement.PlayIdleAnimation();
                Invoke(nameof(GoToSeat), 2f);
                break;

            case CustomerState.MovingToSeat:
                movement.MoveToSeat(() => SetState(CustomerState.Sitting));
                break;

            case CustomerState.Sitting:
                movement.Sit();
                var order = GetComponent<CustomerOrder>();
                if (order != null)
                    order.StartOrderingAfterDelay(2.5f); // 2.5초 후 주문
                break;

            case CustomerState.Leaving:
                movement.ReleaseSeat();
                movement.LeaveStore(() => Destroy(gameObject));
                break;
        }
    }

    private void GoToSeat()
    {
        SetState(CustomerState.MovingToSeat);
    }
}
