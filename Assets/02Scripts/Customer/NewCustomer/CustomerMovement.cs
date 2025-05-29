using DalbitCafe.Customer;
using UnityEditor.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;

// �̵� + ��� + Flip + �ִϸ��̼�
// ���� �մ��� �ֹ� ��/ �ڸ� �̵� ���̸� �� �մ�x
public class CustomerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 target;
    private bool isMoving;
    private Action onArrive;
    [SerializeField] private List<Vector3> path;
    private int pathIndex;
    private List<Vector3> debugPath;


    public void WalkRandomly()
    {
        Vector3 spawnPos = CustomerSpawner.Instance.GetRandomStreetPosition(); // �մ� ��ġ ����
        transform.position = spawnPos;
        Vector3 opposite = CustomerSpawner.Instance.GetOppositeStreetPosition(spawnPos);
        MoveToEntrance(() => CustomerSpawner.Instance.TryEnterCustomer(this)); // ������: �Ա� = �����ߴٸ�
    }

    public void MoveToEntrance(Action onDone)
    {
        Vector3 entrancePos = CustomerSpawner.Instance.GetEntrancePosition();
        MoveTo(entrancePos, onDone);

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
        animator.Play("Back_Idle_Stand");
        //spriteRenderer.flipX = true;
    }

    public void Sit()
    {
        // ���� ���⿡ ����
        animator.Play("Front_Idle_Sit");
        //Front_Sit: �ɴ� �ִϸ��̼�
        //Front_Idle_Sit: �ɾ��ִ� �ִϸ��̼�
    }

    private void MoveTo(Vector3 destination, Action callback)
    {
        onArrive = callback;
        path = PathfindingManager.Instance.FindPath(transform.position, destination);
        debugPath = path; // ��� ����

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("��� ����!");
            isMoving = false;
            return;
        }

        pathIndex = 0;
        isMoving = true;
    }



    private void Update()
    {
        if (!isMoving || path == null || pathIndex >= path.Count) return;

        Vector3 next = path[pathIndex];
        Vector3 dir = (next - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, next) < 0.05f)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                isMoving = false;
                onArrive?.Invoke();
            }
        }

        UpdateAnimation(dir);
    }


    private void UpdateAnimation(Vector3 dir)
    {
        if(dir.y > 0)
        {
            animator.Play("Back_Walk");
            spriteRenderer.flipX = (dir.x > 0 ? false : true);
        }
        else
        {
            animator.Play("Front_Walk");
            spriteRenderer.flipX = (dir.x > 0 ? true : false);

        }
    }

    private void OnDrawGizmos()
    {
        if (debugPath == null || debugPath.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < debugPath.Count - 1; i++)
        {
            Gizmos.DrawLine(debugPath[i], debugPath[i + 1]);
            Gizmos.DrawSphere(debugPath[i], 0.05f);
        }
    }

}
