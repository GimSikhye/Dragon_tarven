using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.5f; //흔들리는 시간
    public float strength = 0.3f; // 흔들리는 세기
    public int vibrato = 10; //진동 횟수 (값에 클수록 자잘하게 흔들림)
    public float randomness = 90f; // 흔들릴 '방향'의 랜덤 정도

    private Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.position;
    }

    public void Shake()
    {
        // 현재 실행 중인 tween이 있다면 정리
        transform.DOKill();

        // ShakePosition으로 만들고, 끝나면 원래 위치로 복귀
        transform.DOShakePosition(duration, strength, vibrato, randomness, false, true)
            .OnComplete(() => transform.position = originalPos);
    }
}
