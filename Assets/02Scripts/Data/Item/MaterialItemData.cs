using UnityEngine;

public enum MaterialType
{
    Drink, // ����(�̴ϰ��ӿ� ���� ���)
    SideMenu, // ������ ���� �����Ҷ� (����� UI�� ��� ����)
    Both // �Ѵ� ���̴� ����(����� ���̵�޴� �Ѵ�)
}

[CreateAssetMenu(menuName = "SO/Inventory/MaterialItem")]
public class MaterialItemData : ItemData
{
    public MaterialType materialType;
    public override ItemCategory Category => ItemCategory.Material;
    public override System.Enum SubCategory => materialType;


}
