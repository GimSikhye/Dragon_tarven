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
        SetState(CustomerState.WalkingAround); // 생성될때 걷기
    }

    public void SetState(CustomerState newState)
    {
        switch(newState)
        {
            case CustomerState.WalkingAround:
                //Debug.Log("주위 걷는중");
                movement.WalkRandomly(); // 랜덤한 자리로 걸음?
                break;

            case CustomerState.Entering: // 가게에 들어섬
                //Debug.Log("가게에 들어가는중");
                movement.MoveToCounter(() => SetState(CustomerState.Ordering)); // () => : SetState(CustomerState.Ordering)을 호출하는 익명함수
                break;

            case CustomerState.Ordering:
                movement.PlayIdleAnimation();
                //Debug.Log("주문 후 자리 이동");
                Invoke(nameof(GoToSeat), 2f); // 2초 후 자리로 이동
                break;

            case CustomerState.MovingToSeat:
                //Debug.Log("자리로 가는중");
                movement.MoveToSeat(() => SetState(CustomerState.Sitting));
                break;

            case CustomerState.Sitting:
                //Debug.Log("앉아있는중");
                movement.Sit();
                break;

            case CustomerState.Leaving:
                //Debug.Log("가게를 떠나는중");
                movement.LeaveStore(() => Destroy(gameObject));
                break;
        }
    }

    private void GoToSeat()
    {
        SetState(CustomerState.MovingToSeat);
    }
}
