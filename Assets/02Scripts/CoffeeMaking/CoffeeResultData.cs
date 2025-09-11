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

    public string EvaluateGrade()
    {
        int correctCount = 0;

        if (BaseMatch) correctCount++;
        if (ShotAccuracy >= 0.9f) correctCount++;  // 샷이 거의 정확히 맞았을 경우 인정
        if (PourAccuracy >= 0.8f) correctCount++;  // 우유량도 비슷하게 맞았을 때만 인정
        if (SyrupMatch) correctCount++;
        if (WhippedMatch) correctCount++;

        // 등급 판정
        return correctCount switch
        {
            0 => "Fail",
            1 or 2 => "Bad",
            3 or 4 => "Good",
            >= 5 => "Perfect",
        };
    }
}
