using UnityEngine;

public class CharacterAnimatorController : MonoBehaviour
{
    // �ִϸ��̼� ���� ���
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private string currentState = "Fonrt_Idle_Stand";
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animator.Play(currentState);
    }

    public void UpdateDirectionAndPlay(Vector2 fromPos, Vector2 toPos)
    {
        Vector2 dir = (toPos - fromPos).normalized;
        string newState = "";

        // ���� ���
        if (dir.y > 0.5f)
        {
            newState = "Back_Walk";
            spriteRenderer.flipX = dir.x < 0;
        }
        else if (dir.y < -0.5f)
        {
            newState = "Front_Walk";
            spriteRenderer.flipX = dir.x > 0;
        }
        else
        {
            // �¿� ���� �̵� �� Y ��ȭ �̹��� ��� -> ���� ���� �������� Walk ����
            if (currentState.StartsWith("Back"))
            {
                newState = "Back_Walk";
                // �¿� �Ǻ��� dir.x �������� �ٽ� �Ǵ�
                spriteRenderer.flipX = dir.x < 0;
            }
            else
            {
                newState = "Front_Walk";
                spriteRenderer.flipX = dir.x > 0;
            }
        }

        // �ߺ� ��ȯ ����
        if (!string.IsNullOrEmpty(newState) && newState != currentState)
        {
            animator.CrossFadeInFixedTime(newState, 0.2f);
            currentState = newState;
        }

    }

    public void PlayIdle()
    {
        string idleState = "";

        switch(currentState)
        {
            case "Back_Walk":
                idleState = "Back_Idle_Stand";
                break;
            case "Front_Walk":
                idleState = "Front_Idle_Stand";
                break;
            default:
                idleState = "Front_Idle_Stand";
                break;
        }

        animator.CrossFadeInFixedTime(idleState, 0.2f);
        currentState = idleState;
    }
}
