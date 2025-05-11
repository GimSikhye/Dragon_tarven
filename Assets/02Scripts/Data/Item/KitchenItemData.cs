using UnityEngine;


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

[CreateAssetMenu(menuName = "SO/Inventory/KitchenItem")]
public class KitchenItemData : ItemData
{
    public KitchenType kitchenType; // SO���� ����

    public override ItemCategory Category => ItemCategory.Kitchen;

    public override System.Enum SubCategory => kitchenType; 
}
