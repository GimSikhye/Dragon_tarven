using DalbitCafe.Customer;
using UnityEditor.Tilemaps;
using UnityEngine;
using System;
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
        MoveTool(opposite, () => CustomerSpawner.Instance.TryEnterCustomer(this));
    }

    public void MoveToCounter(Action onDone)
    {
        Vector3 counterPos = CustomerSpawner.Instance.GetCounterPosition();
        MoveTool(counterPos, onDone);
    }
}
