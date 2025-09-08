using System.Collections.Generic;

[System.Serializable]
public class MenuVariantRecipe
{
    public string variantName; // 변종메뉴 이름
    public string[] hintText; // 힌트들
    public string baseType; // 베이스재료 타입
    public int shotCount; // 샷 개수
    public float expectedPourAmount; // 베이스재료 붓는 양
    public string whippedCreamLevel; // 휘핑크림 높이
    public List<SyrupRequirement> syrups; // 시럽들 필요량
}

[System.Serializable]
public class SyrupRequirement
{
    public string syrupName;
    public int count;
}
