using UnityEngine;
using UnityEngine.AI; // Nav Mesh �� ����ϱ� ���� �ʿ��� using ��

public class Monster : MonoBehaviour
{
    [SerializeField] Transform target; // ���� Ÿ��

    NavMeshAgent agent; // Ž�� �޽� ������Ʈ�� ���� ������ �ʿ�

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Agent �� Target �� ���� �̵��� �� ������ ȸ������
        agent.updateUpAxis = false; // ĳ������ �̵��� ������� �����ϱ� ����
        
    }


    void Update()
    {
        agent.SetDestination(target.position); // Agent���� target�� ���� ��ġ�� �̵��ϵ��� ����
    }



}