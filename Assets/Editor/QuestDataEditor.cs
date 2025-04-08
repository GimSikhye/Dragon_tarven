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

        quest.icon = (Sprite)EditorGUILayout.ObjectField("����Ʈ ������", quest.icon, typeof(Sprite), false);
        quest.questTitle = EditorGUILayout.TextField("����Ʈ ����", quest.questTitle);
        quest.description = EditorGUILayout.TextField("����Ʈ ����", quest.description);

        quest.questType = (QuestType)EditorGUILayout.EnumPopup("����Ʈ Ÿ��", quest.questType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("����Ʈ ���� ���", EditorStyles.boldLabel);

        if (quest.conditions == null || quest.conditions.Length == 0)
        {
            quest.conditions = new QuestCondition[1] { new QuestCondition() };
        }

        for (int i = 0; i < quest.conditions.Length; i++)
        {
            EditorGUILayout.BeginVertical("box");
            var condition = quest.conditions[i];

            condition.type = (QuestConditionType)EditorGUILayout.EnumPopup("���� Ÿ��", condition.type);

            if (condition.type == QuestConditionType.SellItem)
            {
                int selected = Mathf.Max(0, System.Array.IndexOf(coffeeIds, condition.targetItemId));
                selected = EditorGUILayout.Popup("�Ǹ��� Ŀ��", selected, coffeeIds);
                condition.targetItemId = coffeeIds[selected];
            }
            else if (condition.type == QuestConditionType.PlaceFurniture || condition.type == QuestConditionType.UpgradeInterior)
            {
                int selected = Mathf.Max(0, System.Array.IndexOf(furnitureIds, condition.targetItemId));
                selected = EditorGUILayout.Popup("���� ID", selected, furnitureIds);
                condition.targetItemId = furnitureIds[selected];
            }
            else
            {
                condition.targetItemId = EditorGUILayout.TextField("��� ������ ID", condition.targetItemId);
            }

            condition.requiredAmount = EditorGUILayout.IntField("�䱸 ����", condition.requiredAmount);


            if (GUILayout.Button("�� ���� ����"))
            {
                var list = new System.Collections.Generic.List<QuestCondition>(quest.conditions);
                list.RemoveAt(i);
                quest.conditions = list.ToArray();
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("���� �߰�"))
        {
            var list = new System.Collections.Generic.List<QuestCondition>(quest.conditions);
            list.Add(new QuestCondition());
            quest.conditions = list.ToArray();
        }

        EditorGUILayout.Space();
        quest.rewardGold = EditorGUILayout.IntField("����Ʈ ���� ���", quest.rewardGold);
        quest.rewardExp = EditorGUILayout.IntField("����Ʈ ���� ����ġ", quest.rewardExp);
        quest.nextQuest = (QuestData)EditorGUILayout.ObjectField("���� ����Ʈ", quest.nextQuest, typeof(QuestData), false);
        quest.isCompleted = EditorGUILayout.Toggle("�Ϸ��", quest.isCompleted);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(quest);
        }
    }
}
