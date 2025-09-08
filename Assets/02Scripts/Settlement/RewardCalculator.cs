using UnityEngine;

public static class RewardCalculator
{
    private const float BASE_COIN = 50f; // 기본 보상 코인

    public static void GrantCoinByResult(CoffeeResultData result)
    {
        int correctCount = 0;

        if (result.BaseMatch) correctCount++;
        if (result.ShotAccuracy >= 0.9f) correctCount++;
        if (result.PourAccuracy >= 0.9f) correctCount++;
        if (result.SyrupMatch) correctCount++;
        if (result.WhippedMatch) correctCount++;

        float totalReward = 0f;

        if (correctCount == 5)
        {
            totalReward = BASE_COIN * 1.5f; // 50% 보너스
        }
        else if (correctCount >= 2)
        {
            totalReward = BASE_COIN * (correctCount / 5f); // 비율 지급
        }
        else
        {
            totalReward = 0f; // 실패
        }

        if (totalReward > 0)
        {
            PlayerStatsManager.Instance.AddCoin((int)totalReward);
            Debug.Log($"골드 지급: {totalReward} 코인 (정답 수: {correctCount})");
        }
        else
        {
            Debug.Log("정답 수 부족 - 골드 미지급");
        }
    }
}
