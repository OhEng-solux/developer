using UnityEngine;
using UnityEngine.UI;

public class ImagePopupManager : MonoBehaviour
{
    public static ImagePopupManager instance;

    public GameObject popupPanel;     // ImagePopupPanel
    public Image popupImage;          // 자식 오브젝트: Image
    public Button closeButton;        // 자식 오브젝트: Button

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

        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
    }

    /// <summary>
    /// 이미지 팝업을 보여줍니다.
    /// </summary>
    /// <param name="image">보여줄 스프라이트 이미지</param>
    public void ShowPopup(Sprite image)
    {
        if (popupImage != null)
            popupImage.sprite = image;

        if (popupPanel != null)
            popupPanel.SetActive(true);
    }

    /// <summary>
    /// 이미지 팝업을 숨깁니다.
    /// </summary>
    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}
