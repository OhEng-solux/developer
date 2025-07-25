using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public GameObject highlightBorder;
    public Text itemNameText; // 이름 텍스트

    public void SetItem(Item item)
    {
        // 아이콘 이미지 설정 (습득 여부에 따라 분기)
        if (item != null)
        {
            iconImage.sprite = item.isObtained ? item.obtainedIcon : item.silhouetteIcon;
            iconImage.color = Color.white;

            // 이름 설정
            if (itemNameText != null)
            {
                itemNameText.text = item.isObtained ? item.itemName : "???";
            }
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = Color.clear;
            if (itemNameText != null)
            {
                itemNameText.text = "";
            }
        }
    }

    public void SetHighlight(bool isOn)
    {
        highlightBorder.SetActive(isOn);
    }
}
