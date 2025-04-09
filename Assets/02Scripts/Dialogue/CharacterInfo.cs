
using UnityEngine;

public enum CharacterExpression
{
    Default,
    Smile,
    Sad
}

[System.Serializable]
public class CharacterInfo : ScriptableObject
{
    public string characterName;
    public Sprite defaultExpression;
    public Sprite smileExpression;
    public Sprite sadExpression;

    public Sprite GetExpressionSprite(CharacterExpression expression)
    {
        switch (expression)
        {
            case CharacterExpression.Smile:
                return smileExpression;
            case CharacterExpression.Sad:
                return sadExpression;
            case CharacterExpression.Default:
            default:
                return defaultExpression;
        }
    }
}
