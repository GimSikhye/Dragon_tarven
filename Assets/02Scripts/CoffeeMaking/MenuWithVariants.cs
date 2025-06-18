using System.Collections.Generic;

[System.Serializable]
public class MenuWithVariants
{
    public CustomerOrder.MenuType menuType;
    public List<MenuVariantRecipe> variants;
}
