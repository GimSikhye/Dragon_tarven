using UnityEngine;
public enum CharacterExpression
{
    Default,
    Smile,
    Disappointed
}

[System.Serializable] // �ν����Ϳ� �� Ŭ������ ������ ���̰� ����
public class CharacterInfo : ScriptableObject
{
    // ĳ���� ����
    public string characterName;
    // ĳ���� ����ǥ����
    public Sprite defaultSprite;
    public Sprite smileSprite;
    public Sprite disappointedSprite;

    public Sprite GetExpressionSprite(CharacterExpression expression) // ���� ������ ���� ������ ��������Ʈ�� ��ȯ
    {
        return expression switch
        {
            CharacterExpression.Smile => smileSprite,
            CharacterExpression.Disappointed => disappointedSprite,
            _ => defaultSprite,
        };
    }
}
