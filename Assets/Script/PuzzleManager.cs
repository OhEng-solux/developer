using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public List<string> correctAnswer = new List<string> { "○", "△", "□", "☆" };
    private List<string> playerInput = new List<string>();

    public GameObject puzzlePanel;
    public Dialogue successDialogue;
    public Dialogue failDialogue;
    private DialogueManager dm;

    public GameObject keyObject; // 열쇠 프리팹 또는 이미지
    public Light spotlight; // 조명 효과 (Optional)

    private void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
        keyObject.SetActive(false);
    }

    public void ButtonPressed(string shape)
    {
        playerInput.Add(shape);

        if (playerInput.Count == 4)
        {
            if (IsCorrect())
                StartCoroutine(SuccessSequence());
            else
                StartCoroutine(FailSequence());
        }
    }

    bool IsCorrect()
    {
        for (int i = 0; i < 4; i++)
            if (playerInput[i] != correctAnswer[i]) return false;
        return true;
    }

    IEnumerator SuccessSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;
        dm.ShowDialogue(successDialogue);

        keyObject.SetActive(true); // 열쇠 보여주기
        if (spotlight != null) spotlight.intensity = 0.2f; // 조명 어둡게
        // 열쇠 인벤토리에 추가하는 로직도 여기서 호출 가능
    }

    IEnumerator FailSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;
        dm.ShowDialogue(failDialogue);
        playerInput.Clear(); // 초기화
    }
}
