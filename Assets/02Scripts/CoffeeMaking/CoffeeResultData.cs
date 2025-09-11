using UnityEngine;

public class CoffeeResultData
{
    public bool BaseMatch;
    public bool ShotAccuracy;
    public float PourAccuracy;
    public bool SyrupMatch;
    public string WhippedLevel;
    public bool WhippedMatch;

    public string EvaluateGrade()
    {
        int correctCount = 0;

        if (BaseMatch)
        {
            Debug.Log("베이스 정답");
            correctCount++;
        }
        if (ShotAccuracy)
        {
            Debug.Log("샷 정답");
            correctCount++;  // 샷이 거의 정확히 맞았을 경우 인정
        }
        if (PourAccuracy < 5f)
        {
            Debug.Log("pour 오차량 5미만");
            correctCount++;
        }
        if (SyrupMatch)
        {
            Debug.Log("시럽 횟수&종류 정답");
            correctCount++;
        }
        if(WhippedLevel == "")
        {
            correctCount++;

        }
        if (WhippedMatch)
        {
            Debug.Log("휘핑레벨 매치 됨");
            correctCount++;
        }

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
