using UnityEngine;

public class QusetTracker : MonoBehaviour
{
    public void OnCoffeeSold(string coffeeId)
    {
        QuestManager.Instance.CheckQuestProgress(coffeeId, QusetType.SellCoffee);
    }

    public void OnFurniturePlaced(string furnitureId)
    {
        QuestManager.Instance.CheckQuestProgress(furnitureId, QusetType.PlaceFurniture);
    }

    public void OnFurnitureUpgraded(string furnitureId)
    {
        QuestManager.Instance.CheckQuestProgress(furnitureId, QusetType.UpgradeFurniture);
    }
}
