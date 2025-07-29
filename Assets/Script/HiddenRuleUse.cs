using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ItemUse/HiddenRule")]
public class HiddenRuleUse : ItemUseHandler
{
    [SerializeField] private GameObject viewerPanel;  // 쪽지를 보여줄 UI 패널
    [SerializeField] private Image viewerImage;        // 이미지 보여줄 컴포넌트
    [SerializeField] private Sprite hiddenRuleSprite;  // 쪽지 이미지

    private bool isViewing = false;

    public override void Use()
    {
        if (viewerPanel == null || viewerImage == null || hiddenRuleSprite == null)
        {
            Debug.LogWarning("[HiddenRuleUse] UI 요소가 연결되지 않았습니다!");
            return;
        }

        // 이미지 표시
        viewerImage.sprite = hiddenRuleSprite;
        viewerPanel.SetActive(true);
        isViewing = true;

        // 닫기 체크용 코루틴 실행
        CoroutineRunner.Instance.StartCoroutine(WaitForClose());
    }

    private System.Collections.IEnumerator WaitForClose()
    {
        while (isViewing)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Escape))
            {
                viewerPanel.SetActive(false);
                isViewing = false;
            }
            yield return null;
        }
    }
}