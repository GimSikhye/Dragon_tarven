using DG.Tweening;
using TMPro;
using UnityEngine;

public static class TextAnimationHelper
{
    public static void AnimateNumber(TextMeshProUGUI text, float from, float to, float duration = 0.6f)
    {
        DOTween.To(() => from, x => {
            from = x;
            text.text = from.ToString("N0"); // 숫자 포맷 (1,000처럼 쉼표 있음)
        }, to, duration);
    }
}
