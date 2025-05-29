using UnityEngine;

[System.Serializable] // 인스펙터에 이 클래스의 내용을 보이게 해줌
public class CharacterInfo : ScriptableObject
{
    // 캐릭터 정보
    public string characterName;
    public Sprite namePlateSprite; // 이름표 이미지 추가
    public Sprite characterSprite;
}
