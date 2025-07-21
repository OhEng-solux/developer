using UnityEngine;
public enum ItemType
{
    Consumable,  // 소모 아이템
    Permanent    // 영구 아이템
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isObtained;
    public ItemType itemType;
}
