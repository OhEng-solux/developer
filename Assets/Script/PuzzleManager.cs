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

    // ���� Ȱ��ȭ ���� �÷���
    private bool _isPuzzleActive = false;
    private bool _isPuzzleSolved = false; // �߰�: ������ �̹� �����ߴ°�

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
                Debug.LogWarning($"{btnTransform.name}�� AnswerButton ������Ʈ�� �����ϴ�.");
        }
    }


    // ���� ���� �Լ�
    public void StartPuzzle()
    {
        playerInput.Clear();
        puzzlePanel.SetActive(true);
        Time.timeScale = 0f;
        _isPuzzleActive = true;
    }

    public void ButtonPressed(string shape)
    {
        if (!_isPuzzleActive) return; // ���� Ȱ�� ���� ���� �Է�

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
        _isPuzzleSolved = true; // ������ �÷��� true
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
