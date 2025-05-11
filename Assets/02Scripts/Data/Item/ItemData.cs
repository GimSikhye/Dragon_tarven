using UnityEngine;
public enum ItemCategory
{
    Kitchen, // �ֹ�
    Interior, // ���׸���
    Exterior // �ͽ��׸���
}

// ��� �������� �������� ������ ������
[CreateAssetMenu(menuName = "SO/Inventory/ItemData")]
public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public GameObject prefab; // ��ġ�� ������

    public abstract ItemCategory Category { get; }
    public virtual System.Enum SubCategory => null;



}
