using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Button List")]
    public List<Transform> correctAnswerButtons;

    [Header("정답 인덱스 순서! (ex: 2,0,3)")]
    public List<int> answerSequence; // 정답 버튼의 인덱스 순서로 입력

    private List<int> playerInputSequence = new List<int>();

    [Header("UI")]
    public GameObject puzzlePanel;
    public Dialogue successDialogue;
    public Dialogue failDialogue;
    public GameObject keyObject;

    private DialogueManager dm;
    private bool _isPuzzleActive = false;
    private bool _isPuzzleSolved = false;

    private int selectedButtonIndex = 0;
    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    public bool IsPuzzleActive() => _isPuzzleActive;
    public bool IsPuzzleSolved() => _isPuzzleSolved;

    private Color pressedColor = new Color(0.7f, 0.7f, 0.7f); // 어두운 회색 등
    private HashSet<int> pressedIndices = new HashSet<int>();

    private void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
        keyObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isPuzzleActive) return;
        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(1);

        if (Input.GetKeyDown(KeyCode.Return))
            PressSelectedButton();
    }

    void MoveSelection(int direction)
    {
        // 선택 해제 시 이전 색 복구
        // 이미 입력했다면 pressedColor, 아니면 normalColor
        SetButtonColor(selectedButtonIndex, pressedIndices.Contains(selectedButtonIndex) ? pressedColor : normalColor);

        selectedButtonIndex += direction;
        if (selectedButtonIndex < 0) selectedButtonIndex = correctAnswerButtons.Count - 1;
        if (selectedButtonIndex >= correctAnswerButtons.Count) selectedButtonIndex = 0;

        // 현재 커서는 노란색
        SetButtonColor(selectedButtonIndex, selectedColor);
    }

    void SetButtonColor(int index, Color color)
    {
        if (index >= 0 && index < correctAnswerButtons.Count)
        {
            var img = correctAnswerButtons[index].GetComponent<Image>();
            if (img != null)
            {
                img.color = color;
            }
        }
    }

    void PressSelectedButton()
    {
        // 선택 시 pressed로 표시
        SetButtonColor(selectedButtonIndex, pressedColor);
        pressedIndices.Add(selectedButtonIndex);

        playerInputSequence.Add(selectedButtonIndex);

        var btn = correctAnswerButtons[selectedButtonIndex].GetComponent<AnswerButton>();
        if (btn != null && !string.IsNullOrEmpty(btn.button_sound) && AudioManager.instance != null)
            AudioManager.instance.Play(btn.button_sound);

        if (playerInputSequence.Count == answerSequence.Count)
        {
            if (IsCorrect())
                StartCoroutine(SuccessSequence());
            else
                StartCoroutine(FailSequence());
        }
    }

    bool IsCorrect()
    {
        for (int i = 0; i < answerSequence.Count; i++)
            if (playerInputSequence[i] != answerSequence[i]) return false;
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
        _isPuzzleSolved = true;
        playerInputSequence.Clear();
        pressedIndices.Clear();

        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = true;
    }

    IEnumerator FailSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        puzzlePanel.SetActive(false);
        DialogueManager.instance.ShowDialogue(failDialogue);
        _isPuzzleActive = false;
        playerInputSequence.Clear();
        pressedIndices.Clear();
        Time.timeScale = 1f;

        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = true;
    }


    public void StartPuzzle()
    {
        playerInputSequence.Clear();
        pressedIndices.Clear();
        puzzlePanel.SetActive(true);
        Time.timeScale = 0f;
        _isPuzzleActive = true;

        selectedButtonIndex = 0;
        for (int i = 0; i < correctAnswerButtons.Count; i++)
            SetButtonColor(i, i == selectedButtonIndex ? selectedColor : normalColor);

        // 플레이어 이동 차단
        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = false;
    }

}
