using UnityEngine;

// ����Ʈ ������ �����ϰ� QuestManager�� �����ϴ� ������ ��.
public class QuestTracker : MonoBehaviour
{
    public void OnCoffeeSold(string coffeeId)
    {
        GameManager.Instance.QuestManager.CheckQuestProgress(coffeeId, QuestConditionType.SellItem); 
    }

    public void OnFurniturePlaced(string furnitureId)
    {
        GameManager.Instance.QuestManager.CheckQuestProgress(furnitureId, QuestConditionType.PlaceFurniture); 
    }

    public void OnFurnitureUpgraded(string furnitureId)
    {
        GameManager.Instance.QuestManager.CheckQuestProgress(furnitureId, QuestConditionType.UpgradeInterior); 
    }
}
