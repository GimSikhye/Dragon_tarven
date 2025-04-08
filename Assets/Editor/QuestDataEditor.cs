using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestData))]
public class QuestDataEditor : Editor
{
    private string[] coffeeIds = new string[] { "americano", "latte", "espresso" };
    private string[] furnitureIds = new string[] { "chair_01", "table_01", "counter_01" };

    public override void OnInspectorGUI()
    {
        QuestData quest = (QuestData)target;

        quest.icon = (Sprite)EditorGUILayout.ObjectField("퀘스트 아이콘", quest.icon, typeof(Sprite), false);
        quest.questTitle = EditorGUILayout.TextField("퀘스트 제목", quest.questTitle);
        quest.description = EditorGUILayout.TextField("퀘스트 설명", quest.description);

        quest.questType = (QuestType)EditorGUILayout.EnumPopup("퀘스트 타입", quest.questType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("퀘스트 조건 목록", EditorStyles.boldLabel);

        if (quest.conditions == null || quest.conditions.Length == 0)
        {
            quest.conditions = new QuestCondition[1] { new QuestCondition() };
        }

        for (int i = 0; i < quest.conditions.Length; i++)
        {
            EditorGUILayout.BeginVertical("box");
            var condition = quest.conditions[i];

            condition.type = (QuestConditionType)EditorGUILayout.EnumPopup("조건 타입", condition.type);

            if (condition.type == QuestConditionType.SellItem)
            {
                int selected = Mathf.Max(0, System.Array.IndexOf(coffeeIds, condition.targetItemId));
                selected = EditorGUILayout.Popup("판매할 커피", selected, coffeeIds);
                condition.targetItemId = coffeeIds[selected];
            }
            else if (condition.type == QuestConditionType.PlaceFurniture || condition.type == QuestConditionType.UpgradeInterior)
            {
                int selected = Mathf.Max(0, System.Array.IndexOf(furnitureIds, condition.targetItemId));
                selected = EditorGUILayout.Popup("가구 ID", selected, furnitureIds);
                condition.targetItemId = furnitureIds[selected];
            }
            else
            {
                condition.targetItemId = EditorGUILayout.TextField("대상 아이템 ID", condition.targetItemId);
            }

            condition.requiredAmount = EditorGUILayout.IntField("요구 수량", condition.requiredAmount);


            if (GUILayout.Button("이 조건 삭제"))
            {
                var list = new System.Collections.Generic.List<QuestCondition>(quest.conditions);
                list.RemoveAt(i);
                quest.conditions = list.ToArray();
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("조건 추가"))
        {
            var list = new System.Collections.Generic.List<QuestCondition>(quest.conditions);
            list.Add(new QuestCondition());
            quest.conditions = list.ToArray();
        }

        EditorGUILayout.Space();
        quest.rewardGold = EditorGUILayout.IntField("퀘스트 보상 골드", quest.rewardGold);
        quest.rewardExp = EditorGUILayout.IntField("퀘스트 보상 경험치", quest.rewardExp);
        quest.nextQuest = (QuestData)EditorGUILayout.ObjectField("다음 퀘스트", quest.nextQuest, typeof(QuestData), false);
        quest.isCompleted = EditorGUILayout.Toggle("완료됨", quest.isCompleted);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(quest);
        }
    }
}
