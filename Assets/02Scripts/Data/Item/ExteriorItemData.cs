using UnityEngine;

public enum ExteriorType
{
    SecondFloorOnly, // 2������
    OutdoorDecoration, // �߿����ǰ
    WallExteriorDecoration, // �ǹ� �ܺ� ���
    Railing2F, // 2�� ����
    Stair2F, // 2�� ���
    WallExterior, // �ǹ� �ܺ�
    Entrance // �Ա�
}

[CreateAssetMenu(menuName = "SO/Inventory/ExteriorItem")]
public class ExteriorItemData : ItemData
{
    public ExteriorType exteriorType; // ���� Ÿ��
    public override ItemCategory Category => ItemCategory.Exterior;
}
