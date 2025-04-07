using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.5f; //��鸮�� �ð�
    public float strength = 0.3f; // ��鸮�� ����
    public int vibrato = 10; //���� Ƚ�� (���� Ŭ���� �����ϰ� ��鸲)
    public float randomness = 90f; // ��鸱 '����'�� ���� ����

    private Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.position;
    }

    public void Shake()
    {
        // ���� ���� ���� tween�� �ִٸ� ����
        transform.DOKill();

        // ShakePosition���� �����, ������ ���� ��ġ�� ����
        transform.DOShakePosition(duration, strength, vibrato, randomness, false, true)
            .OnComplete(() => transform.position = originalPos);
    }
}
