using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Answer")]
    public List<Transform> correctAnswerButtons;

    private List<string> correctAnswer = new List<string>();
    private List<string> playerInput = new List<string>();

    [Header("UI")]
    public GameObject puzzlePanel;
    public Dialogue successDialogue;
    public Dialogue failDialogue;
    public GameObject keyObject;

    private DialogueManager dm;

    // 퍼즐 활성화 상태 플래그
    private bool _isPuzzleActive = false;
    private bool _isPuzzleSolved = false; // 추가: 퍼즐이 이미 성공했는가

    public bool IsPuzzleActive() => _isPuzzleActive;
    public bool IsPuzzleSolved() => _isPuzzleSolved;

    private void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
        keyObject.SetActive(false);

        correctAnswer.Clear();
        foreach (Transform btnTransform in correctAnswerButtons)
        {
            AnswerButton btn = btnTransform.GetComponent<AnswerButton>();
            if (btn != null)
                correctAnswer.Add(btn.shapeValue);
            else
                Debug.LogWarning($"{btnTransform.name}에 AnswerButton 컴포넌트가 없습니다.");
        }
    }


    // 퍼즐 시작 함수
    public void StartPuzzle()
    {
        playerInput.Clear();
        puzzlePanel.SetActive(true);
        Time.timeScale = 0f;
        _isPuzzleActive = true;
    }

    public void ButtonPressed(string shape)
    {
        if (!_isPuzzleActive) return; // 퍼즐 활성 중일 때만 입력

        playerInput.Add(shape);

        if (playerInput.Count == correctAnswer.Count)
        {
            if (IsCorrect())
                StartCoroutine(SuccessSequence());
            else
                StartCoroutine(FailSequence());
        }
    }

    bool IsCorrect()
    {
        for (int i = 0; i < correctAnswer.Count; i++)
            if (playerInput[i] != correctAnswer[i]) return false;
        return true;
    }

    IEnumerator SuccessSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;
        dm.ShowDialogue(successDialogue);
        keyObject.SetActive(true);

        _isPuzzleActive = false;
        _isPuzzleSolved = true; // 성공시 플래그 true
        playerInput.Clear();
    }

    IEnumerator FailSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);

        puzzlePanel.SetActive(false);

        DialogueManager.instance.ShowDialogue(failDialogue);

        _isPuzzleActive = false;
        playerInput.Clear();

        Time.timeScale = 1f;
    }

}
