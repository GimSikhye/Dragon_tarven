using DalbitCafe.Customer;
using UnityEditor.Tilemaps;
using UnityEngine;
using System;

// 이동 + 경로 + Flip + 애니메이션
// 기존 손님이 주문 중/ 자리 이동 중이면 새 손님x
public class CustomerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 target;
    private bool isMoving;
    private Action onArrive;

    public void WalkRandomly()
    {
        Vector3 spawnPos = CustomerSpawner.Instance.GetRandomStreetPosition();
        transform.position = spawnPos;
        Vector3 opposite = CustomerSpawner.Instance.GetOppositeStreetPosition(spawnPos);
        MoveTo(opposite, () => CustomerSpawner.Instance.TryEnterCustomer(this));
    }

    public void MoveToCounter(Action onDone)
    {
        Vector3 counterPos = CustomerSpawner.Instance.GetCounterPosition();
        MoveTo(counterPos, onDone);
    }

    public void MoveToSeat(Action onDone)
    {
        Vector3 seatPos = CustomerSpawner.Instance.GetAvailableSeatPosition();
        MoveTo(seatPos, onDone);
    }

    public void LeaveStore(Action onDone)
    {
        Vector3 exit = CustomerSpawner.Instance.GetRandomStreetPosition();
        MoveTo(exit, onDone);
    }

    public void PlayIdleAnimation()
    {
        animator.Play("Front_Idle_Stand");
    }

    public void Sit()
    {
        // 의자 방향에 따라서
        animator.Play("Sit");
        //Front_Sit: 앉는 애니메이션
        //Front_Idle_Sit: 앉아있는 애니메이션
    }

    private void MoveTo(Vector3 destination, Action callback)
    {
        target = destination;
        onArrive = callback;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving) return;

        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime; //MoveToWard를 안쓰는 이유?

        if(Vector3.Distance(transform.position, target) < 0.1f)
        {
            isMoving = false;
            onArrive?.Invoke();
        }

        UpdateAnimation(dir);
    }

    private void UpdateAnimation(Vector3 dir)
    {
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            animator.Play("Walk_Side");
            spriteRenderer.flipX = dir.x < 0;
        }
        else
        {
            if (dir.y > 0) animator.Play("Walk_Back");
            else animator.Play("Walk_Front");
        }
    }
}
