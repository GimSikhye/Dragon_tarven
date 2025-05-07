using System.Collections.Generic;
using UnityEngine;

public class DragJudge : MonoBehaviour
{
    public StainPatternGenerator generator;
    public float shapeMatchThreshold = 1.5f; // DTW 거리 기준값 (실험적으로 조정 가능)
    public int resampleCount = 32;

    public void Evaluate(List<Vector3> drawn)
    {
        if (drawn == null || drawn.Count < 2)
        {
            Debug.LogWarning("Too few points drawn — evaluation skipped.");
            return;
        }

        List<Vector3> pattern = generator.GetCurrentPatternPoints();
        if (pattern == null || pattern.Count < 2)
        {
            Debug.LogWarning("Pattern path too short — evaluation skipped.");
            return;
        }

        int sampleCount = Mathf.Min(resampleCount, drawn.Count, pattern.Count);
        List<Vector3> patternNorm = NormalizeAndResample(pattern, sampleCount);
        List<Vector3> drawnNorm = NormalizeAndResample(drawn, sampleCount);

        float dtwDist = ComputeDTWDistance(drawnNorm, patternNorm);

        bool isShapeMatch = dtwDist < shapeMatchThreshold;
        string result = isShapeMatch ? "PERFECT" : "BAD";

        Debug.Log($"Result: {result}, DTW Distance: {dtwDist:F5}, SampleCount: {sampleCount}");
        Debug.Log($"Pattern normalized points: {string.Join(",", patternNorm)}");
        Debug.Log($"Drawn normalized points: {string.Join(",", drawnNorm)}");
    }

    List<Vector3> NormalizeAndResample(List<Vector3> points, int count)
    {
        List<Vector3> resampled = Resample(points, count);
        Vector3 center = GetCentroid(resampled);
        float scale = GetMaxExtent(resampled, center);

        for (int i = 0; i < resampled.Count; i++)
        {
            resampled[i] = (resampled[i] - center) / scale;
        }

        return resampled;
    }

    List<Vector3> Resample(List<Vector3> points, int count)
    {
        List<Vector3> result = new();
        if (points.Count < 2)
        {
            for (int i = 0; i < count; i++) result.Add(points[0]);
            return result;
        }

        float totalLength = 0f;
        List<float> segmentLengths = new();
        for (int i = 1; i < points.Count; i++)
        {
            float segLen = Vector3.Distance(points[i - 1], points[i]);
            segmentLengths.Add(segLen);
            totalLength += segLen;
        }

        float step = totalLength / (count - 1);
        float distSoFar = 0f;
        result.Add(points[0]);

        int currSeg = 0;
        while (result.Count < count && currSeg < segmentLengths.Count)
        {
            float segLen = segmentLengths[currSeg];
            Vector3 p0 = points[currSeg];
            Vector3 p1 = points[currSeg + 1];

            while (distSoFar + step <= segLen)
            {
                distSoFar += step;
                float t = distSoFar / segLen;
                result.Add(Vector3.Lerp(p0, p1, t));
            }

            distSoFar -= segLen;
            currSeg++;
        }

        if (result.Count < count) result.Add(points[^1]);
        return result;
    }

    Vector3 GetCentroid(List<Vector3> points)
    {
        Vector3 sum = Vector3.zero;
        foreach (var p in points) sum += p;
        return sum / points.Count;
    }

    float GetMaxExtent(List<Vector3> points, Vector3 center)
    {
        float max = 0f;
        foreach (var p in points)
        {
            float dist = (p - center).magnitude;
            if (dist > max) max = dist;
        }
        return max > 0f ? max : 1f;
    }

    float ComputeDTWDistance(List<Vector3> a, List<Vector3> b)
    {
        int n = a.Count;
        int m = b.Count;
        float[,] dtw = new float[n + 1, m + 1];

        const float INF = float.PositiveInfinity;

        for (int i = 0; i <= n; i++)
            for (int j = 0; j <= m; j++)
                dtw[i, j] = INF;

        dtw[0, 0] = 0f;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                float cost = Vector3.Distance(a[i - 1], b[j - 1]);
                dtw[i, j] = cost + Mathf.Min(dtw[i - 1, j], dtw[i, j - 1], dtw[i - 1, j - 1]);
            }
        }

        return dtw[n, m];
    }
}
