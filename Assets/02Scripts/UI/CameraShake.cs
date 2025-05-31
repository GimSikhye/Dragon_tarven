using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class CameraShake : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 45f; // »ÁµÈ∏≤ ºº±‚ («»ºø ¥‹¿ß)
    private float dampingSpeed = 1.0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Vector2 offset = Random.insideUnitCircle * shakeMagnitude;
            rectTransform.anchoredPosition = originalPosition + (Vector3)offset;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void Shake(float duration = 0.3f, float magnitude = 5f)
    {
        originalPosition = rectTransform.anchoredPosition;
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
