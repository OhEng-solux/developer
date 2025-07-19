using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public GameObject highlightBorder;

    public void SetItem(Item item)
    {
        iconImage.sprite = item.icon;
        iconImage.color = item.isObtained ? new Color(1,1,1,1f) : new Color(1,1,1,0.3f);
    }

    public void SetHighlight(bool isOn)
    {
        highlightBorder.SetActive(isOn);
    }
}
