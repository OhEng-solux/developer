using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // 중복 방지
    }

    public GameObject menuPanel; // 메뉴 전체 패널
    public AudioManager theAudio;

    public string call_sound;
    public string cancel_sound;
    public string select_sound;

    public OrderManager theOrder;

    public List<Button> menuButtons; // 메뉴 항목 버튼들 (인스펙터에 할당)

    public bool activated;
    public bool closePopup=false;
    private int currentIndex = 0; // 현재 선택된 버튼 인덱스

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
            bool popupInactive = PopupManager.instance == null || !PopupManager.instance.IsPopupActive();
            bool saveInactive = SaveManager.instance == null || !SaveManager.instance.IsSaveActive();
            bool inventoryInactive = InventoryManager.instance == null || !InventoryManager.instance.IsInventoryActive();

            if (popupInactive && saveInactive && inventoryInactive)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("SaveManager.instance: " + (SaveManager.instance != null));
                    Debug.Log("IsSaveActive Open: " + SaveManager.instance?.IsSaveActive());
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
        StartCoroutine(ResetInputsNextFrame());
    }

    IEnumerator ResetInputsNextFrame()
    {
        yield return null; // 한 프레임 대기
        Input.ResetInputAxes();
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

            cb.normalColor = (i == currentIndex) ? Color.white : Color.gray;

            // 선택된 버튼을 강조하고, 나머지는 비활성 색으로
            cb.highlightedColor = cb.normalColor;
            cb.pressedColor = cb.normalColor;
            cb.selectedColor = cb.normalColor;

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
                    closePopup = true;
                }
            );
    }
}
