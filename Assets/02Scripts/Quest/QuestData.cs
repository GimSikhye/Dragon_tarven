using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "SO/QuestData")]
public class QuestData : ScriptableObject
{
    public Sprite icon;
    public string questTitle;
    public string description;

    [System.Serializable]
    public class QuestCondition
    {
        public QusetType type;
        public string targetItemId;
        public int requiredAmount;
        [HideInInspector] public int currentAmount;
        public int rewardGold;
        public int rewardExp;
    }

    public List<QuestCondition> conditions = new();
    public QuestData nextQuest; // ¿¬°è Äù½ºÆ®
    public bool isCompleted;
}
