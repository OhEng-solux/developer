using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button[] buttons;   // Inspector에 버튼 3개 연결
    public GameObject[] highlightImgs;
    public GameObject savePanel;
    public GameObject icon;
    public GameObject Panel;
    private int selectedIndex = 0;

    private AudioManager theAudio;
    public string keySound;
    public string enterSound;

    void Start()
    {
        theAudio = FindFirstObjectByType<AudioManager>();

        HighlightButton();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex + 3 - 1) % 3;
            theAudio.Play(keySound);
            HighlightButton();

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) %3;
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
        for (int i = 0; i < highlightImgs.Length; i++)
        {
            highlightImgs[i].SetActive(i == selectedIndex);
        }
    }
    void OnSelect(int idx)
    {
        for(int i = 0; i < 3; i++)
        {
            //buttons[i].gameObject.SetActive(false);
        }
        //icon.gameObject.SetActive(false);
        Panel.gameObject.SetActive(false);
        switch (idx)
        {
          
            case 0: // 처음부터 시작
                Debug.Log("처음부터 시작!");

                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); // 원하는 게임씬명으로
                break;
            case 1: //  SavePanel 팝업
                Debug.Log("세이브 패널 팝업!");
                savePanel.SetActive(true);
                break;
            case 2: //  종료
                Debug.Log("게임 종료!");
                Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false; // 에디터에서는 이 명령 필요
                break;
        }
    }
}