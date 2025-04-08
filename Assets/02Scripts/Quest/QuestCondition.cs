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
    public string targetItemId;        // � ����������
    public int requiredAmount;         // �� ���� �Ⱦƾ� �ϴ���
    public int currentAmount;          // ���� �Ǹ��� ���� (�ǽð� ������Ʈ��)

    public QuestConditionType type;    // ���� Ÿ�� (��: ������ �Ǹ�, ���� ��ġ ��)
    public int rewardGold;             // ���Ǻ� ���� ��� (�ʿ��� ���)
    public int rewardExp;
}
