using UnityEngine;
public enum ItemCategory
{
    Kitchen, // �ֹ�
    Interior, // ���׸���
    Exterior, // �ͽ��׸���
    Material, // ���(�丮 ����)
    Food
}
// ������, ��, �ٴ�, ���̺�
public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price; // ���Ĵ� ����
    public GameObject prefab; // ��ġ�� ������(���� �������� ���)

    public abstract ItemCategory Category { get; }
    public virtual System.Enum SubCategory => null; // default



}
