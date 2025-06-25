using UnityEngine;
public enum QuestConditionType
{
    SellItem,
    PlaceFurniture,
    UpgradeInterior
}

[System.Serializable]
public class QuestCondition // 퀘스트 조건
{
    public string targetItemId;        // 어떤 아이템인지
    public int requiredAmount;         // 조건에 필요한 수량
    public int currentAmount;          // 현재 충족된 개수

    public QuestConditionType type;    // 조건 타입 (예: 아이템 판매, 가구 배치 등)

}
