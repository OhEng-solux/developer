using UnityEngine;

public class KeyUseManager : MonoBehaviour
{
    [SerializeField] private GameObject transferObject; // 지하실로 이동할 수 있는 오브젝트
    [SerializeField] private GameObject blockerObject; // 막는 Collider 오브젝트

    void Start()
    {
        // 열쇠 사용 전: 이동 막기 & 벽 활성화
        if (transferObject != null)
            transferObject.SetActive(false);

        if (blockerObject != null)
            blockerObject.SetActive(true);
    }

    public void TryUseBasementKey()
    {
        if (transferObject != null)
            transferObject.SetActive(true); // 이동 가능하게 변경

        if (blockerObject != null)
            blockerObject.SetActive(false); // 벽 제거

        PopupManager.instance?.ShowPopup("문이 열렸습니다!");
    }
}
