using UnityEngine;

public class FlipXDebugger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool lastFlipX;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        lastFlipX = spriteRenderer.flipX;
    }

    private void Update()
    {
        if (spriteRenderer.flipX != lastFlipX)
        {
            Debug.Log($"[FlipXDebugger] FlipX 변경됨: {lastFlipX} → {spriteRenderer.flipX} at {Time.time:F3}초");

            // 호출 스택 찍기 (누가 바꿨는지 힌트 제공)
            Debug.Log(new System.Diagnostics.StackTrace().ToString());


            lastFlipX = spriteRenderer.flipX;
        }
    }
}
