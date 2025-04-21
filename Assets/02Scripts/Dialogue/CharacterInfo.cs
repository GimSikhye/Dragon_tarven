using UnityEngine;

public enum CharacterExpression
{
    Default,
    Smile,
    Disappointed
}

[System.Serializable]
public class CharacterInfo : ScriptableObject
{
    // 감정표현 sprite들
    public string characterName;
    public Sprite defaultSprite;
    public Sprite smileSprite;
    public Sprite disappointedSprite;

    public Sprite GetExpressionSprite(CharacterExpression expression)
    {
        return expression switch
        {
            CharacterExpression.Smile => smileSprite,
            CharacterExpression.Disappointed => disappointedSprite,
            _ => defaultSprite,
        };
    }
}
