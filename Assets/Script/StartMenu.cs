using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button[] buttons;   // Inspector�� ��ư 3�� ����
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
          
            case 0: // ó������ ����
                Debug.Log("ó������ ����!");

                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); // ���ϴ� ���Ӿ�������
                break;
            case 1: //  SavePanel �˾�
                Debug.Log("���̺� �г� �˾�!");
                savePanel.SetActive(true);
                break;
            case 2: //  ����
                Debug.Log("���� ����!");
                Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ����� �� ��� �ʿ�
                break;
        }
    }
}