using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        Panel.gameObject.SetActive(false);
        switch (idx)
        {
          
            case 0: // ó������ ����
                Debug.Log("ó������ ����");
                SceneManager.LoadScene("Prologue");
                break;
            case 1: //  SavePanel �˾�
                Debug.Log("���̺� �г� �˾�");//�ҷ����� ����
                savePanel.SetActive(true);
                break;
            case 2: //  ����
                Debug.Log("���� ����");
                Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ����� �� ��� �ʿ�
                break;
        }
    }
}