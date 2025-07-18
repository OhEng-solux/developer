using UnityEngine;
using UnityEngine.UI;

public class NameInputManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject nameInputPanel;
    public InputField nameInputField;
    public DialogueManager dialogueManager;// Inspector¿¡ ¿¬°á
    void Start()
    {
        nameInputField.text = "";
        nameInputPanel.SetActive(false);
    }

    public void ShowPanel()
    {
        nameInputPanel.SetActive(true);
        nameInputField.text = "";
        nameInputField.ActivateInputField();
    }

    public void HidePanel()
    {
        nameInputPanel.SetActive(false);
    }

    public void OnSubmitName()
    {
        string inputName = nameInputField.text.Trim();
        if (!string.IsNullOrEmpty(inputName))
        {
            dialogueManager.OnNameInputCompleted(inputName);
            HidePanel();
        }
    }
}
