using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private string keySound = "type_Sound";
    private string enterSound = "enter_Sound";
    private string openSound = "ok_Sound";
    private string beepSound = "beep_Sound";
    private AudioManager audioManager;
    public GameObject menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(true);
        }
    }
}
