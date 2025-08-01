using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public Text timeText;
    public Text dateText;
    public Text dayText;
    public Text nameText;
    public GameObject highlightBorder;

    public void SetData(SaveNLoad.Data data)
    {
        if (data == null)
        {
            nameText.text = "---";//여기에 이름
            dateText.text = "---";
            timeText.text = "---";
            dayText.text = "---";
            return;
        }


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

    public void SetSaveInfo(string characterName, string saveDate, string saveTime,string sceneName)
    {
        nameText.text = characterName;
        dayText.text = sceneName;
        dateText.text = saveDate;
        timeText.text = saveTime;
    }

    public void SetEmpty()
    {
        nameText.text = "비어 있음";
        dateText.text = "";
        timeText.text = "";

    }
}