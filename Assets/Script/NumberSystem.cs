using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumberSystem : MonoBehaviour
{
    private AudioManager theAudio;

    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string correct_sound;

    [SerializeField] private GameObject lockUIPanel; // 자물쇠 이미지가 포함된 캔버스 패널
    [SerializeField] private Text[] numberTexts; // 4자리 텍스트 UI

    [SerializeField] private SpriteRenderer targetRenderer; // 바꿔줄 대상
    [SerializeField] private Sprite successSprite;           // 정답 맞췄을 때의 이미지
    [SerializeField] private Sprite failSprite;

    private int[] currentDigits = new int[4];
    private int selectedIndex = 0;
    private int correctNumber = 2265;

    public bool activated { get; private set; } = false;
    private bool correctFlag = false;
    private bool wasCancelled = false;
    private bool inputEnabled = false;

    void Start()
    {
        theAudio = FindFirstObjectByType<AudioManager>();
    }

    void Update()
    {
        if (!activated || !inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex = (selectedIndex + 1) % 4;
            theAudio.Play(key_sound);
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex = (selectedIndex + 3) % 4;
            theAudio.Play(key_sound);
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentDigits[selectedIndex] = (currentDigits[selectedIndex] + 1) % 10;
            theAudio.Play(key_sound);
            UpdateNumberTexts();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentDigits[selectedIndex] = (currentDigits[selectedIndex] + 9) % 10;
            theAudio.Play(key_sound);
            UpdateNumberTexts();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            theAudio.Play(enter_sound);
            CheckAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            theAudio.Play(cancel_sound);
            CancelInput();
        }
    }

    public void ShowNumber(int _correctNumber)
    {
        correctNumber = _correctNumber;
        currentDigits = new int[4];
        selectedIndex = 0;
        activated = true;
        inputEnabled = true;
        correctFlag = false;
        wasCancelled = false;

        UpdateNumberTexts();
        UpdateHighlight();

        if (lockUIPanel != null)
            lockUIPanel.SetActive(true);

        gameObject.SetActive(true);
        PlayerManager.instance.canMove = false;
    }

    private void CheckAnswer()
    {
        int result = currentDigits[3] * 1000 + currentDigits[2] * 100 + currentDigits[1] * 10 + currentDigits[0];
        correctFlag = (result == correctNumber);
        Debug.Log($"[NumberSystem] 입력: {result}, 정답: {correctNumber}, 결과: {correctFlag}");

        theAudio.Play(correctFlag ? correct_sound : cancel_sound);

        if (targetRenderer != null)
        {
            if (correctFlag && successSprite != null)
            {
                targetRenderer.sprite = successSprite;
                Debug.Log("[NumberSystem] 정답 → 성공 스프라이트로 변경됨");
            }
            else if (!correctFlag && failSprite != null)
            {
                targetRenderer.sprite = failSprite;
                Debug.Log("[NumberSystem] 오답 → 실패 스프라이트로 변경됨");
            }
        }

        StartCoroutine(ExitPuzzleRoutine());
    }

    private void CancelInput()
    {
        wasCancelled = true;
        correctFlag = false;
        StartCoroutine(ExitPuzzleRoutine());
    }

    private IEnumerator ExitPuzzleRoutine()
    {
        // 정답 판정 이후 변경된 이미지 확인을 위한 지연 시간
        yield return new WaitForSeconds(1.0f); 

        activated = false;
        inputEnabled = false;

        if (lockUIPanel != null)
            lockUIPanel.SetActive(false);

        PlayerManager.instance.canMove = true;
    }

    public bool GetResult()
    {
        return correctFlag;
    }

    public bool WasCancelled()
    {
        return wasCancelled;
    }

    private void UpdateNumberTexts()
    {
        for (int i = 0; i < 4; i++)
        {
            numberTexts[i].text = currentDigits[i].ToString();
        }
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < 4; i++)
        {
            Color color = numberTexts[i].color;
            color.a = (i == selectedIndex) ? 1f : 0.4f;
            numberTexts[i].color = color;
        }
    }
}