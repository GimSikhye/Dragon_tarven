using DG.Tweening;
using UnityEngine;

public class UIShake : MonoBehaviour
{
    [SerializeField] private Transform uiRoot;
    [Header("Shake Settings")]
    public float duration = 0.5f;
    public float strength = 0.3f;
    public int vibrato = 10; // Number of vibrations
    public float randomness = 90f; // The randomness of the 'direction' of shaking

    public void ShakeUI()
    {
        uiRoot.DOKill();
        uiRoot.DOShakePosition(duration, new Vector3(strength, strength, 0), vibrato, randomness, false, true)
              .OnComplete(() => uiRoot.localPosition = Vector3.zero);
    }

}
