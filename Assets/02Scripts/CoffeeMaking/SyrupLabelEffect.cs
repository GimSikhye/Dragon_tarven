using UnityEngine;
using TMPro;

public class SyrupLabelEffect : MonoBehaviour
{
    [SerializeField] private float floatUpDistance = 50f;
    [SerializeField] private float duration = 1f;

    private Vector3 startPos;
    private float elapsed = 0f; // 경과

    public void Play()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float t = elapsed += Time.deltaTime;
        transform.position = startPos + Vector3.up * floatUpDistance * t;

        // 점점 투명하게
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        if(text != null)
        {
            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            text.color = c;
        }

        // 일정 시간 후 파괴
        if(elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
