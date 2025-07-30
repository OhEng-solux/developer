using UnityEngine;
using UnityEngine.UI;

public class ImagePopupManager : MonoBehaviour
{
    public static ImagePopupManager instance;

    public GameObject popupPanel;
    public Image popupImage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    private void Update()
    {
        if (popupPanel != null && popupPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                HidePopup();
            }
        }
    }

    public void ShowPopup(Sprite image)
    {
        if (popupImage != null)
            popupImage.sprite = image;

        if (popupPanel != null)
            popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public bool IsImageActive() //팝업이 떠있는지 외부에서 확인할 수 있도록 함
    {
        Debug.Log("popupPanel.activeSelf" + popupPanel.activeSelf);
        return popupPanel.activeSelf;
    }
}
