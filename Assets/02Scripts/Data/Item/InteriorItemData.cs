using UnityEngine;

public enum InteriorType
{
    Table, // ���̺�
    Chair, // ����
    Partition, // ��Ƽ��
    Decoration, // ���ǰ 
    BeanContainer, // ������
    WallDecoration, // �����
    Tile, // Ÿ��
    Wallpaper // ����
}

[CreateAssetMenu(menuName = "SO/Inventory/InteriorItem")]
public class InteriorItemData : ItemData
{
    public InteriorType interiorType;

    public override ItemCategory Category => ItemCategory.Interior;
    public override System.Enum SubCategory => interiorType;
}
