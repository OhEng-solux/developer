using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    public GameObject go;
    public AudioManager theAudio;

    public string call_sound;
    public string cancel_sound;

    public OrderManager theOrder;

    private bool activated;

    void Start()
    {
        activated = false;
        go.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Continue()
    {
        activated = false;
        go.SetActive(false);
        Time.timeScale = 1f;

        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = true;

        theAudio.Play(cancel_sound);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            activated = !activated;

            if (activated)
            {
                go.SetActive(true);
                Time.timeScale = 0f;

                if (PlayerManager.instance != null)
                    PlayerManager.instance.canMove = false;

                theAudio.Play(call_sound);
            }
            else
            {
                go.SetActive(false);
                Time.timeScale = 1f;

                if (PlayerManager.instance != null)
                    PlayerManager.instance.canMove = true;

                theAudio.Play(cancel_sound);
            }
        }
    }
}
