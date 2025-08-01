using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartNEndMenu : MonoBehaviour
{
    public static StartNEndMenu instance;

    public Button[] buttons;   // Inspector에 버튼 3개 연결
    public GameObject savePanel;
    public GameObject icon;
    public GameObject Panel;
    private int selectedIndex = 0;

    private AudioManager theAudio;
    public string keySound;
    public string enterSound;

    private Color selectedColor = Color.white;
    private Color unselectedColor = Color.gray;

    private bool isQuitPopupActive = false; // 종료 팝업 활성 상태 플래그

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        theAudio = FindFirstObjectByType<AudioManager>();
        HighlightButton();
    }

    void Update()
    {
        // Debug.Log("현재 씬 이름: " + gameObject.scene.name);

        // 종료 팝업이 활성 상태면 ESC로 팝업 닫기, 그 외 입력 무시
        if (isQuitPopupActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PopupManager.instance.ClosePopup();
                isQuitPopupActive = false;

                Panel.SetActive(true);
                selectedIndex = 0;
                HighlightButton();

                Debug.Log("종료 팝업 ESC로 닫힘 - 메뉴 복귀");
            }
            return;
        }

        // SavePanel이 켜져있으면 ESC 눌러서 세이브 창 닫고 메뉴판넬 보여주기
        if (SaveManager.instance != null && SaveManager.instance.IsSaveActive())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveManager.instance.CloseSave();
                Panel.SetActive(true);
                selectedIndex = 0;
                HighlightButton();
                return;
            }
            // 세이브 창 열렸을 때는 메뉴 이동이나 선택 무시
            return;
        }

        // End 씬 대화 중일 경우 메뉴 비활성화
        if (gameObject.scene.name == "Ending_Bad" && (DialogueManager.instance == null || DialogueManager.instance.talking))
        {
            Panel.gameObject.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex + buttons.Length - 1) % buttons.Length;
            theAudio.Play(keySound);
            HighlightButton();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % buttons.Length;
            theAudio.Play(keySound);
            HighlightButton();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            theAudio.Play(enterSound);
            OnSelect(selectedIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameObject.scene.name != "Start")
            {
                Debug.Log("ESC 눌림 - 메뉴로 복귀");
                Panel.SetActive(true);
                selectedIndex = 0;
                HighlightButton();
            }
        }
    }

    void HighlightButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock cb = buttons[i].colors;
            cb.normalColor = (i == selectedIndex) ? selectedColor : unselectedColor;
            cb.highlightedColor = cb.normalColor;
            cb.pressedColor = cb.normalColor;
            cb.selectedColor = cb.normalColor;
            buttons[i].colors = cb;
        }
    }

    void OnSelect(int idx)
    {
        if (gameObject.scene.name == "Start")
        {
            switch (idx)
            {
                case 0: // 처음부터 시작
                    Debug.Log("처음부터 시작");
                    SceneManager.LoadScene("Prologue");
                    break;

                case 1: // 세이브 패널 열기
                    Debug.Log("세이브 패널 팝업");
                    SaveManager.instance.StartCoroutine(SaveManager.instance.OpenSave());
                    Panel.gameObject.SetActive(false);
                    break;

                case 2: // 종료
                    Debug.Log("게임 종료 선택됨");

                    isQuitPopupActive = true; // 종료 팝업 활성 상태로 변경

                    PopupManager.instance.ShowChoicePopup(
                        "게임을 종료하시겠습니까?",
                        () => {
                            Application.Quit();
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#endif
                        },
                        () => {
                            // '아니오' 선택 시 팝업 닫고 메뉴 복귀
                            PopupManager.instance.ClosePopup();

                            isQuitPopupActive = false;

                            Panel.SetActive(true);
                            selectedIndex = 0;
                            HighlightButton();

                            Debug.Log("종료 취소 > 메뉴 복귀");
                        }
                    );
                    break;

            }
        }
        else
        {
            // Start 씬이 아닐 때 메뉴 동작
            switch (idx)
            {
                case 0: // 다시하기 (세이브 패널 열기)
                    Debug.Log("다시 시작");
                    SaveManager.instance.StartCoroutine(SaveManager.instance.OpenSave());
                    Panel.gameObject.SetActive(false);
                    break;

                case 1: // 메인으로 돌아가기
                    Debug.Log("메인으로");
                    SceneManager.LoadScene("Start");
                    break;
            }
        }
    }

    public bool IsPanelActive()
    {
        Debug.Log("IsPanelActive: " + Panel.activeSelf);
        return Panel.activeSelf;
    }

    public void ShowPanel()
    {
        Panel.SetActive(true);
        selectedIndex = 0;
        HighlightButton();
    }
}
