using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel; // 메뉴 전체 패널
    public AudioManager theAudio;

    public string call_sound;
    public string cancel_sound;
    public string select_sound;

    public OrderManager theOrder;

    public List<Button> menuButtons; // 메뉴 항목 버튼들 (인스펙터에 할당)

    private bool activated;

    private int currentIndex = 0; // 현재 선택된 버튼 인덱스
    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    void Start()
    {
        activated = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        // 버튼 색상 초기화
        UpdateButtonColors();
    }

    void Update()
    {
        if (DialogueManager.instance == null || !DialogueManager.instance.talking)
        {
            // ESC 키로 메뉴 켜고 끄기
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                activated = !activated;

                if (activated)
                {
                    OpenMenu();
                }
                else
                {
                    CloseMenu();
                }
            }

            if (activated)
            {
                HandleInput();
            }
        }
    }

    void OpenMenu()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;

        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = false;

        theAudio.Play(call_sound);

        currentIndex = 0;
        UpdateButtonColors();
    }

    void CloseMenu()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = true;

        theAudio.Play(cancel_sound);

    }

    void HandleInput()
    {
        // 방향키 위/아래로 메뉴 선택 변경
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = menuButtons.Count - 1;

            theAudio.Play(select_sound);
            UpdateButtonColors();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex++;
            if (currentIndex >= menuButtons.Count)
                currentIndex = 0;

            theAudio.Play(select_sound);
            UpdateButtonColors();
        }

        // 선택 확정 : 스페이스 또는 엔터
        if (Input.GetKeyDown(KeyCode.Return))
        {
            theAudio.Play(select_sound);
            ClickCurrentButton();
        }
    }

    void UpdateButtonColors()
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            ColorBlock cb = menuButtons[i].colors;
            if (i == currentIndex)
            {
                cb.normalColor = selectedColor;
                cb.highlightedColor = selectedColor;
            }
            else
            {
                cb.normalColor = normalColor;
                cb.highlightedColor = normalColor;
            }
            menuButtons[i].colors = cb;
        }
    }

    void ClickCurrentButton()
    {
        if (currentIndex >= 0 && currentIndex < menuButtons.Count)
        {
            menuButtons[currentIndex].onClick.Invoke();
        }
        CloseMenu();
    }

    // 기존 함수들 
    public void Exit()
    {
        Application.Quit();
    }

    public void Continue()
    {
        activated = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        theAudio.Play(cancel_sound);
        CloseMenu();
    }

    public void LoadStartScene()
    {
        PopupManager.instance.ShowChoicePopup(
                "메뉴 화면으로 이동하시겠습니까?",
                () =>
                {
                    // 사용자가 '예' 눌렀을 경우 씬 이동
                    Debug.Log("메뉴 이동 확인");
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
                },
                () =>
                {
                    // 사용자가 '아니오' 눌렀을 경우 취소 처리만
                    Debug.Log("메뉴 이동 취소");
                    // 팝업 닫으면 메뉴 그대로 유지 가능
                }
            );
    }
}
