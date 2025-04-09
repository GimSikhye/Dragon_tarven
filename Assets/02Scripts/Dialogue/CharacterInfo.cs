using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "SO/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
}
