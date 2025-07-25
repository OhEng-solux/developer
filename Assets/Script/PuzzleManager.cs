using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Button List")]
    public List<Transform> correctAnswerButtons;

    [Header("���� �ε��� ����! (ex: 2,0,3)")]
    public List<int> answerSequence; // ���� ��ư�� �ε��� ������ �Է�

    private List<int> playerInputSequence = new List<int>();

    [Header("UI")]
    public GameObject puzzlePanel;
    public Dialogue successDialogue;
    public Dialogue failDialogue;
    public GameObject keyObject;
    [Header("Item")]
    private InventoryManager theInventory;
    [SerializeField] private Item rewardItem; // 보상 아이템


    private DialogueManager dm;
    private bool _isPuzzleActive = false;
    private bool _isPuzzleSolved = false;

    private int selectedButtonIndex = 0;
    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    public bool IsPuzzleActive() => _isPuzzleActive;
    public bool IsPuzzleSolved() => _isPuzzleSolved;

    private Color pressedColor = new Color(0.7f, 0.7f, 0.7f); // ��ο� ȸ�� ��
    private HashSet<int> pressedIndices = new HashSet<int>();

    private void Start()
    {
        dm = FindFirstObjectByType<DialogueManager>();
        keyObject.SetActive(false);
        theInventory = FindFirstObjectByType<InventoryManager>(); // InventoryManager 연결
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
        // ���� ���� �� ���� �� ����
        // �̹� �Է��ߴٸ� pressedColor, �ƴϸ� normalColor
        SetButtonColor(selectedButtonIndex, pressedIndices.Contains(selectedButtonIndex) ? pressedColor : normalColor);

        selectedButtonIndex += direction;
        if (selectedButtonIndex < 0) selectedButtonIndex = correctAnswerButtons.Count - 1;
        if (selectedButtonIndex >= correctAnswerButtons.Count) selectedButtonIndex = 0;

        // ���� Ŀ���� �����
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
        // ���� �� pressed�� ǥ��
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
        dm.ShowDialogue(successDialogue); // 대화 출력
        keyObject.SetActive(true); // 시각적 보상 오브젝트 표시

        // 실제 아이템 획득 처리
        if (rewardItem != null && theInventory != null)
        {
            theInventory.AcquireItem(rewardItem); // 아이템 지급
            Debug.Log("[퍼즐 성공] 아이템 지급 완료: " + rewardItem.itemName);
        }

        // 상태 초기화
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

        // �÷��̾� �̵� ����
        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = false;
    }

}
