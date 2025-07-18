using UnityEngine;
using UnityEngine.UI;

public class NameInputField : MonoBehaviour
{
    public InputField inputField; // 유니티 UI InputField
    private PlayerManager thePlayer;
    void Start()
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
        inputField.ActivateInputField(); // 바로 타이핑 가능하게
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string enteredName = inputField.text.Trim();

            if (!string.IsNullOrEmpty(enteredName))
            {
                thePlayer.characterName = enteredName;

                DialogueManager.instance.OnNameInputCompleted(enteredName);

                inputField.gameObject.SetActive(false);
            }
        }
    }
}
