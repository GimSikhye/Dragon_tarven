using UnityEngine;
// 실시간 조건 체크 (판매, 배치 등 이벤트 수신)
// 커피 판매&가구 설치 시점에서 QuesetTracker.Onxxx를 호출해주면 됨.
public class QusetTracker : MonoBehaviour
{
    public void OnCoffeeSold(string coffeId)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        questManager.CheckQuestProgress(coffeId, QusetType.SellCoffee);
    }

    public void OnFuniturePlaced(string furnitureId)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        questManager.CheckQuestProgress(furnitureId, QusetType.PlaceFurniture);
    }
    
    public void OnFurnitureUpgraded(string furnitureId)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        questManager.CheckQuestProgress(furnitureId, QusetType.UpgradeFurniture);
    }

}
