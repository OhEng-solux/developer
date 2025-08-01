using UnityEngine;

public class KeyUseManager : MonoBehaviour
{
    [SerializeField] private GameObject transferObject; // 지하실로 이동할 수 있는 오브젝트
    [SerializeField] private GameObject blockerObject; // 막는 Collider 오브젝트 (=아이템 사용 위치 감지 영역)
    private bool playerInZone = false; // 열쇠 사용 가능 영역에 들어와 있는가?
    void Start()
    {
        if (transferObject == null)
            transferObject = this.gameObject;

        // 열쇠 사용 전: 이동 막기 & 벽 활성화
        if (transferObject != null)
            transferObject.SetActive(false);

        if (blockerObject != null)
            blockerObject.SetActive(true);
    }

    // 플레이어가 영역 안에 들어왔는지 체크
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInZone = false;
    }

    public void TryUseBasementKey()
    {
        if (!playerInZone)
        {
            Debug.Log("[KeyUseManager] 열쇠 사용 불가 - 범위 밖");
            PopupManager.instance?.ShowPopup("아무 일도 일어나지 않았습니다.");
            return;
        }

        if (transferObject != null)
            transferObject.SetActive(true); // 이동 가능하게 변경

        if (blockerObject != null)
            blockerObject.SetActive(false); // 벽 제거

        AudioManager.instance.Play("key_use");
        PopupManager.instance?.ShowPopup("문이 열렸습니다.");
    }
}
