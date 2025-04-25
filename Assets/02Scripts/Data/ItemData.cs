using UnityEngine;
public enum ItemCategory
{
    Kitchen, // �ֹ�
    Interior, // ���׸���
    Exterior // �ͽ��׸���
}

public enum KitchenType
{
    RoastingMachine, // �ν��øӽ�
    CoffeeMachine, // Ŀ�Ǹӽ�
    Workbench, // �۾���
    CookingMachine, // ��ŷ�ӽ�
    Showcase, // �����̽�
    Counter, // ����
    Mixer // �ͼ���
}

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

[CreateAssetMenu(menuName = "SO/Inventory/ItemData")]
public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public GameObject prefab; // ��ġ�� ������

    public abstract ItemCategory Category { get; }

}
