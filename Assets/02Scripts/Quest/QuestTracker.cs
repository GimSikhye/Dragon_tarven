using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public void OnCoffeeSold(string coffeeId)
    {
        QuestManager.Instance.CheckQuestProgress(coffeeId, QuestConditionType.SellItem); 
    }

    public void OnFurniturePlaced(string furnitureId)
    {
        QuestManager.Instance.CheckQuestProgress(furnitureId, QuestConditionType.PlaceFurniture); 
    }

    public void OnFurnitureUpgraded(string furnitureId)
    {
        QuestManager.Instance.CheckQuestProgress(furnitureId, QuestConditionType.UpgradeInterior); 
    }
}
