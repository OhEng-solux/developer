using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HiddenRuleViewer : MonoBehaviour
{
    public static HiddenRuleViewer Instance;

    [Header("UI 요소")]
    [SerializeField] private GameObject viewerPanel;     // 쪽지 패널
    [SerializeField] private Image viewerImage;          // 쪽지 이미지
    [SerializeField] private Sprite hiddenRuleSprite;    // 보여줄 쪽지 이미지

    private bool isViewing = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OpenViewer()
    {
        if (viewerPanel == null || viewerImage == null || hiddenRuleSprite == null)
        {
            Debug.LogWarning("[HiddenRuleViewer] UI 구성요소가 누락되었습니다.");
            return;
        }

        viewerImage.sprite = hiddenRuleSprite; //쪽지 이미지 설정

        RectTransform rt = viewerPanel.GetComponent<RectTransform>();
        Vector2 anchoredPos = rt.anchoredPosition;
        anchoredPos.y = 0f;
        rt.anchoredPosition = anchoredPos;

        viewerPanel.SetActive(true); // 패널 활성화
        isViewing = true;

        StartCoroutine(WaitForClose()); // 닫기 대기 코루틴
    }

    private IEnumerator WaitForClose()
    {
        while (isViewing)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                viewerPanel.SetActive(false);
                isViewing = false;
            }
            yield return null;
        }
    }
}