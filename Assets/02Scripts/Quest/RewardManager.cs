using UnityEngine;

public class RewardManager : MonoSingleton<RewardManager>   
{
    public void GiveReward(int gold, int exp)
    {
        PlayerStatsManager.Instance.AddCoin(gold);
        PlayerStatsManager.Instance.AddExp(exp);
    }
}
