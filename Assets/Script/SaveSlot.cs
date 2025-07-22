using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public Text timeText;
    public Text dateText;
    public Text currentPlaceText;
    public GameObject highlightBorder;

    public void SetData(SaveNLoad.Data data)
    {
        if (data == null)
        {
            currentPlaceText.text = "---";
            dateText.text = "---";
            timeText.text = "---";
            return;
        }

        currentPlaceText.text = data.mapName;

        if (!string.IsNullOrEmpty(data.saveDate))
        {

            dateText.text = data.saveDate;
            timeText.text = data.saveTime;
        }
        else
        {
            dateText.text = "---";
            timeText.text = "---";
        }
    }

    public void SetHighlight(bool isOn)
    {
        highlightBorder.SetActive(isOn);
    }

    public void SetSaveInfo(string mapName, string saveDate, string saveTime)
    {
        currentPlaceText.text = mapName;
        dateText.text = saveDate;
        timeText.text = saveTime;
    }

    public void SetEmpty()
    {
        currentPlaceText.text = "비어있음";
        dateText.text = "";
        timeText.text = "";
    }
}