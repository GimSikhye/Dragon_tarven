using System.Collections.Generic;

[System.Serializable]
public class PlacedItemData
{
    public string itemId;      // ItemData의 이름(고유 식별자)
    public float posX, posY;   // 위치
    public int rotationIndex;  // 회전 인덱스
}

[System.Serializable]
public class PlacedItemSaveData
{
    public List<PlacedItemData> placedItems = new();
}
