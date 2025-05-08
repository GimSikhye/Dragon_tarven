using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.5f; 
    public float strength = 0.3f; 
    public int vibrato = 10; // Number of vibrations
    public float randomness = 90f; // The randomness of the 'direction' of shaking

    private Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.position;
    }

    public void Shake()
    {
        // If there is a tween currently running, delete
        transform.DOKill();

        // When finished, return to original position
        transform.DOShakePosition(duration, strength, vibrato, randomness, false, true)
            .OnComplete(() => transform.position = originalPos);
    }
}
