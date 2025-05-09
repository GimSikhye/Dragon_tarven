using UnityEngine;

public class CharacterAnimatorController : MonoBehaviour
{
    // 애니메이션 제어 담당
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

        // 방향 계산
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
            // 좌우 평행 이동 등 Y 변화 미미할 경우 -> 이전 방향 기준으로 Walk 유지
            if (currentState.StartsWith("Back"))
            {
                newState = "Back_Walk";
                // 좌우 판별은 dir.x 기준으로 다시 판단
                spriteRenderer.flipX = dir.x < 0;
            }
            else
            {
                newState = "Front_Walk";
                spriteRenderer.flipX = dir.x > 0;
            }
        }

        // 중복 전환 방지
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
