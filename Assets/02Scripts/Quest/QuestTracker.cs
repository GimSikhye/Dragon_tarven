using UnityEngine;

// 퀘스트 조건을 추적하고 QuestManager에 전달하는 역할을 함.
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
