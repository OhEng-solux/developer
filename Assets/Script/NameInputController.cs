using UnityEngine;
using UnityEngine.UI;

public class NameInputController : MonoBehaviour
{
    public GameObject nameInputPanel;
    public InputField nameInputField;

    private DialogueManager theDM;
    private bool hasEnteredName = false;

    void Start()
    {
        theDM = DialogueManager.instance;
        theDM.OnSentenceFinished += HandleSentenceFinished;
    }

    private void HandleSentenceFinished(int sentenceIndex)
    {
        if (sentenceIndex == 3 && !hasEnteredName)
        {
            ShowNameInputPanel();
            theDM.PauseDialogue(); // �Ͻ� ����
        }
    }

    private void ShowNameInputPanel()
    {
        nameInputPanel.SetActive(true);
        nameInputField.text = "";
        nameInputField.ActivateInputField();
    }

    private void HideNameInputPanel()
    {
        nameInputPanel.SetActive(false);
    }

    public void OnNameInputCompleted()
    {
        string inputName = nameInputField.text.Trim();
        if (!string.IsNullOrEmpty(inputName))
        {
            PlayerManager.instance.characterName = inputName;
            hasEnteredName = true;
            HideNameInputPanel();

            theDM.ContinueDialogue(); // ���� ��� ����
        }
    }
}
