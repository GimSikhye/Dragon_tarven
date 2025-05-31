using DG.Tweening;
using TMPro;
using UnityEngine;

public static class TextAnimationHelper
{
    public static void AnimateNumber(TextMeshProUGUI text, float from, float to, float duration = 0.6f)
    {
        DOTween.To(() => from, x => {
            from = x;
            text.text = from.ToString("N0"); // ���� ���� (1,000ó�� ��ǥ ����)
        }, to, duration);
    }
}
