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
    public Sprite obtainedIcon;   // 습득 후 이미지
    public Sprite silhouetteIcon; // 습득 전 실루엣 이미지
    public bool isObtained; // 습득 여부 판단
    public ItemType itemType; // 일회용/다회용 구분
    [TextArea] public string description; // 아이템 설명 
}