using UnityEngine;

public class CoffeeResultData
{
    public bool BaseMatch;
    public float ShotAccuracy;
    public float PourAccuracy;
    public int SyrupCount;
    public string WhippedLevel;
    public bool SyrupMatch;
    public bool WhippedMatch;

    public float CalculateScore()
    {
        float score = 0f;
        if (BaseMatch) score += 10f;
        score += ShotAccuracy * 30f;
        score += PourAccuracy * 30f;
        if (SyrupMatch) score += 20f;
        if (WhippedMatch) score += 10f;

        return Mathf.Clamp(score, 0f, 100f);
    }
}
