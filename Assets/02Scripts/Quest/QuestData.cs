using UnityEngine;
using UnityEngine.Rendering.LookDev;
// 퀘스트 데이터 정의 (조건, 보상 등)
public enum QusetType {  SellCoffee, PlaceFurniture, UpgradeFurniture}

[CreateAssetMenu(fileName = "QuestData", menuName = "SO/QuestData")]
public class QuestData : ScriptableObject
{
    public string questTitle;
    public string description;
    public QusetType questType;

    public string targetItemId; // 커피 종류 ID 또는 가구 ID
    public int requiredAmount;

    public int rewardGold;
    public int rewardExp;

    public bool isCompleted;

}
