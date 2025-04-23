using UnityEngine;
public enum CharacterExpression
{
    Default,
    Smile,
    Disappointed
}

[System.Serializable] // 인스펙터에 이 클래스의 내용을 보이게 해줌
public class CharacterInfo : ScriptableObject
{
    // 캐릭터 정보
    public string characterName;
    // 캐릭터 감정표현들
    public Sprite defaultSprite;
    public Sprite smileSprite;
    public Sprite disappointedSprite;

    public Sprite GetExpressionSprite(CharacterExpression expression) // 현재 감정에 따라 적절한 스프라이트를 반환
    {
        return expression switch
        {
            CharacterExpression.Smile => smileSprite,
            CharacterExpression.Disappointed => disappointedSprite,
            _ => defaultSprite,
        };
    }
}
