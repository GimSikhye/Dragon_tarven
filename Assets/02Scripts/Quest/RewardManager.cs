using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public void GiveReward(int gold, int exp)
    {
        GameManager.Instance.PlayerStatsManager.AddCoin(gold);
        GameManager.Instance.PlayerStatsManager.AddExp(exp);
    }
}
