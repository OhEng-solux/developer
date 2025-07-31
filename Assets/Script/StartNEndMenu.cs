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
        Debug.Log("현재 씬 이름: " + gameObject.scene.name);

        if (gameObject.scene.name=="Ending_Bad"&&(DialogueManager.instance == null || DialogueManager.instance.talking))
        {
            Panel.gameObject.SetActive(false);
            return; // 대화 중이면 더 이상 진행 X
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
            Panel.gameObject.SetActive(false);
            switch (idx)
            {
                case 0: // 처음부터 시작
                    Debug.Log("처음부터 시작");
                    SceneManager.LoadScene("Prologue");
                    break;
                case 1: // SavePanel 팝업
                    Debug.Log("세이브 패널 팝업");
                    SaveManager.instance.StartCoroutine(SaveManager.instance.OpenSave());
                    break;
                case 2: // 종료
                    Debug.Log("게임 종료");
                    Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                    break;
            }
        }
        else
        {
            //Panel.gameObject.SetActive(false);
            switch (idx)
            {
                case 0: // 다시하기
                    Debug.Log("다시 시작");
                    SaveManager.instance.StartCoroutine(SaveManager.instance.OpenSave());
                    Panel.gameObject.SetActive(false);
                    break;
                case 1: // 
                    Debug.Log("메인으로");
                    SceneManager.LoadScene("Start");
                    break;
            }
        }
    }

}
