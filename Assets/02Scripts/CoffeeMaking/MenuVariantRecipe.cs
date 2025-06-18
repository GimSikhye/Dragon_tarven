using System.Collections.Generic;

[System.Serializable]
public class MenuVariantRecipe
{
    public string variantName;
    public string[] hintText;
    public string baseType;
    public int shotCount;
    public float expectedPourAmount;
    public string whippedCreamLevel;
    public List<SyrupRequirement> syrups;
}

[System.Serializable]
public class SyrupRequirement
{
    public string syrupName;
    public int count;
}
