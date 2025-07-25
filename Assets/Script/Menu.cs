using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel; // �޴� ��ü �г�
    public AudioManager theAudio;

    public string call_sound;
    public string cancel_sound;
    public string select_sound;

    public OrderManager theOrder;

    public List<Button> menuButtons; // �޴� �׸� ��ư�� (�ν����Ϳ� �Ҵ�)

    private bool activated;

    private int currentIndex = 0; // ���� ���õ� ��ư �ε���
    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    void Start()
    {
        activated = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        // ��ư ���� �ʱ�ȭ
        UpdateButtonColors();
    }

    void Update()
    {
        if (DialogueManager.instance == null || !DialogueManager.instance.talking)
        {
            // ESC Ű�� �޴� �Ѱ� ����
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
        // ����Ű ��/�Ʒ��� �޴� ���� ����
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

        // ���� Ȯ�� : �����̽� �Ǵ� ����
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

    // ���� �Լ��� 
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
                "�޴� ȭ������ �̵��Ͻðڽ��ϱ�?",
                () =>
                {
                    // ����ڰ� '��' ������ ��� �� �̵�
                    Debug.Log("�޴� �̵� Ȯ��");
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
                },
                () =>
                {
                    // ����ڰ� '�ƴϿ�' ������ ��� ��� ó����
                    Debug.Log("�޴� �̵� ���");
                    // �˾� ������ �޴� �״�� ���� ����
                }
            );
    }
}
