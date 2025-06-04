using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Shop Items")]
    [SerializeField] private List<ShopItemData> allItems;

    public List<ShopItemData> GetItems(ShopCategoryType category, DecoSubCategory? sub = null)
    {
        if (category == ShopCategoryType.Decoration && sub.HasValue) // �����۵� �� ī�װ��� Deco�̰�, ����ī�װ� ���ڰ� null�� �ƴ� ��
        {
            return allItems.Where(i => i.category == category && i.subCategory == sub.Value).ToList(); // ShopItemData�� ����� ī�װ���(�μ��� ī�װ��� ����, ����ī�װ��� ���ڿ� �������� List�� ��ȯ 
        }
        return allItems.Where(i => i.category == category).ToList(); // ���� ī�װ��� null �̰ų�, ShopCategoryType�� Deco�� �ƴ϶��, ���ڿ� ���� ī�װ��� ���� �����۵��� ����Ʈ�� ��ȯ
    }

    private void Awake()
    {
        Instance = this;
    }
}
