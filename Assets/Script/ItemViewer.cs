using UnityEngine;
using UnityEngine.UI;

public class ItemViewer : MonoBehaviour
{
    public static ItemViewer instance;

    [SerializeField] private GameObject viewerPanel;     // 전체 패널
    [SerializeField] private Image viewerImage;          // 패널 안 이미지
    [SerializeField] private Sprite hiddenRuleSprite;    // Hidden_Rule 이미지

    private bool isViewing = false;

    void Awake()
    {
        instance = this;
        viewerPanel.SetActive(false);
    }

    void Update()
    {
        if (isViewing && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseNote();
        }
    }

    public void ShowNote()
    {
        viewerImage.sprite = hiddenRuleSprite;
        viewerPanel.SetActive(true);
        isViewing = true;
    }

    public void CloseNote()
    {
        viewerPanel.SetActive(false);
        isViewing = false;
    }
}