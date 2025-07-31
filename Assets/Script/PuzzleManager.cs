using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Button List")]
    public List<Transform> correctAnswerButtons;

    [Header("정답 (ex: 2,0,3)")]
    public List<int> answerSequence;

    private List<int> playerInputSequence = new List<int>();

    [Header("UI")]
    public GameObject puzzlePanel;
    public Dialogue successDialogue;
    public Dialogue failDialogue;

    [Header("Item")]
    private InventoryManager theInventory;
    [SerializeField] private Item rewardItem;

    private DialogueManager dm;
    private bool _isPuzzleActive = false;
    private bool _isPuzzleSolved = false;

    private int selectedButtonIndex = 0;
    private HashSet<int> pressedIndices = new HashSet<int>();

    [Header("선택 강조 투명도 (0.0 ~ 1.0)")]
    [Range(0f, 1f)] public float selectedAlpha = 0.6f;

    public bool IsPuzzleActive() => _isPuzzleActive;
    public bool IsPuzzleSolved() => _isPuzzleSolved;

    private void Start()
    {
        dm = FindFirstObjectByType<DialogueManager>();
        theInventory = FindFirstObjectByType<InventoryManager>();
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
        if (Input.GetKeyDown(KeyCode.Return)) PressSelectedButton();
    }

    void MoveSelection(int direction)
    {
        // 이전 버튼이 눌린 게 아니라면 투명하게 만들기
        if (!pressedIndices.Contains(selectedButtonIndex))
            SetButtonVisual(selectedButtonIndex, selectedAlpha, false);

        selectedButtonIndex += direction;
        if (selectedButtonIndex < 0) selectedButtonIndex = correctAnswerButtons.Count - 1;
        if (selectedButtonIndex >= correctAnswerButtons.Count) selectedButtonIndex = 0;

        // 새 선택 버튼이 눌린 버튼이 아니라면 완전 불투명하게 강조
        if (!pressedIndices.Contains(selectedButtonIndex))
            SetButtonVisual(selectedButtonIndex, 1f, false);
    }

    void SetButtonVisual(int index, float alpha, bool pressed = false)
    {
        if (index >= 0 && index < correctAnswerButtons.Count)
        {
            var img = correctAnswerButtons[index].GetComponent<Image>();
            var btn = correctAnswerButtons[index].GetComponent<AnswerButton>();
            if (img != null && btn != null)
            {
                img.sprite = pressed ? btn.pressedSprite : btn.normalSprite;
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }

    void PressSelectedButton()
    {
        if (pressedIndices.Contains(selectedButtonIndex))
        {
            SetButtonVisual(
                selectedButtonIndex,
                selectedButtonIndex == this.selectedButtonIndex ? 1f : selectedAlpha,
                false
            );
            pressedIndices.Remove(selectedButtonIndex);
            playerInputSequence.RemoveAll(x => x == selectedButtonIndex);
        }
        else
        {
            SetButtonVisual(selectedButtonIndex, 1f, true);
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

        if (rewardItem != null && theInventory != null)
        {
            theInventory.AcquireItem(rewardItem);
            Debug.Log("[퍼즐 성공] 아이템 지급 완료: " + rewardItem.itemName);
        }

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
        {
            if (i == selectedButtonIndex)
                SetButtonVisual(i, 1f, false); // 선택된 버튼은 완전 불투명
            else
                SetButtonVisual(i, selectedAlpha, false); // 나머지는 투명도 적용
        }

        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = false;
    }
}
