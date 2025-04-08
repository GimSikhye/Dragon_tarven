using UnityEngine;
public enum QuestConditionType
{
    SellItem,
    PlaceFurniture,
    UpgradeInterior
}

[System.Serializable]
public class QuestCondition
{
    public string targetItemId;        // 어떤 아이템인지
    public int requiredAmount;         // 몇 개를 팔아야 하는지
    public int currentAmount;          // 현재 판매한 개수 (실시간 업데이트용)

    public QuestConditionType type;    // 조건 타입 (예: 아이템 판매, 가구 배치 등)
    public int rewardGold;             // 조건별 보상 골드 (필요한 경우)
    public int rewardExp;
}
