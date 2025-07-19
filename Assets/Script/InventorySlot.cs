using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public GameObject highlightBorder;
    public Text itemNameText;         // 이름 텍스트

    public void SetItem(Item item)
    {
        iconImage.sprite = item.icon;
        iconImage.color = item.isObtained ? new Color(1,1,1,1f) : new Color(1,1,1,0.3f); //투명도 조절
        // 이름 설정
        if (itemNameText != null)
        {
            itemNameText.text = item.isObtained ? item.itemName : "???";
        }
    }

    public void SetHighlight(bool isOn)
    {
        highlightBorder.SetActive(isOn);
    }
}
