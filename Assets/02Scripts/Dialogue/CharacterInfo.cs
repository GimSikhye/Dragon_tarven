
using UnityEngine;

public enum CharacterExpression
{
    Default,
    Smile,
    Disappointed
}


[CreateAssetMenu(fileName = "New Character", menuName = "SO/Character Info")]
public class CharacterInfo : ScriptableObject
{
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
            _ => defaultSprite
        };
    }
}
