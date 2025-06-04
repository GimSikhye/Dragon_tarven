using UnityEngine;

[CreateAssetMenu(menuName = "SO/Shop/Item")]
public class ShopItemData : ScriptableObject
{
    public string itemName; // ������ �̸�
    [TextArea] public string description; // ������ ���� (TextArea �Ӽ��� ���� �ν����Ϳ��� �ٹٲ��� ������ �Է� �ʵ� ����
    public Sprite icon; // ������ ������
    public int price; // ������ ����
    public ShopCategoryType category; // ī�װ�
    public DecoSubCategory subCategory; //  Decoration �����ۿ��� �ش�Ǵ� ���� ī�װ�

    public ItemData itemData; //������ ������
}
