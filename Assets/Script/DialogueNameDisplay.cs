using UnityEngine;
using UnityEngine.UI;

public class DialogueNameDisplay : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private EndingManager endingManager;

    [SerializeField] private int startIndex = 7;
    [SerializeField] private int endIndex = 9;
    [SerializeField] private string characterName = "행인";

    void Update()
    {
        // 대화가 진행 중인 경우에만 체크
        if (endingManager != null && endingManager.IsDialoguePlaying())
        {
            int idx = endingManager.GetCurrentSentenceIndex();

            if (idx >= startIndex && idx <= endIndex)
            {
                nameText.gameObject.SetActive(true);
                nameText.text = characterName;
            }
            else
            {
                nameText.gameObject.SetActive(false);
            }
        }
        else
        {
            nameText.gameObject.SetActive(false);
        }
    }
}
