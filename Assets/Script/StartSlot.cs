using UnityEngine;
using UnityEngine.UI;
public class StartSlot : MonoBehaviour
{
    public GameObject highlightBorder;

    public void SetHighlight(bool isOn)
    {
        highlightBorder.SetActive(isOn);
    }

  
}