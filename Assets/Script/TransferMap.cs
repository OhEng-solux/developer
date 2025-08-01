using System.Collections;
using UnityEngine;

public class TransferMap : MonoBehaviour
{
    public string transferMapName;
    public Transform target;
    public PolygonCollider2D targetBound;

    private CameraManager theCamera;
    private PlayerManager thePlayer;
    private FadeManager theFade;
    private OrderManager theOrder;
    private GameObject playerLight;

    // 태주 참조
    private TaejuChase chase;

    void Awake()
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
        theCamera = FindFirstObjectByType<CameraManager>();
        theFade = FindFirstObjectByType<FadeManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
        chase = FindFirstObjectByType<TaejuChase>();
        playerLight = GameObject.Find("Light");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("[TransferMap] PLAY 이동 - 코루틴 시작");
            StartCoroutine(TransferCoroutine());

            // 태주가 추격 중이면, 2초 후 이 트리거로 워프
            if (chase != null && chase.IsChasing())
            {
                StartCoroutine(MoveTaejuAfterDelay(3f));
            }
        }
    }

    IEnumerator TransferCoroutine()
    {
        theOrder.NotMove();
        theFade.FadeOut();
        if (chase != null) chase.isFading = true;

        yield return new WaitForSeconds(1f);

        // 플레이어 이동 처리
        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.position.x, target.position.y, theCamera.transform.position.z);
        thePlayer.transform.position = target.position;

        // === 바운드를 이용한 태주 단발 워프 ===
        Bound targetBoundObj = targetBound.GetComponent<Bound>();
        if (chase != null && chase.IsChasing() && targetBoundObj != null)
        {
            // 2초 뒤에 한 번만 소환
            StartCoroutine(MoveTaejuToBoundAfterDelay(chase, targetBoundObj, 3f));
        }

        if (playerLight != null)
            playerLight.SetActive(transferMapName.Trim().ToLower() == "basement");

        theFade.FadeIn();
        yield return new WaitForSeconds(1f);

        if (chase != null) chase.isFading = false;
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }

    private IEnumerator MoveTaejuToBoundAfterDelay(TaejuChase chase, Bound bound, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 taejuSpawn = bound.GetTaejuSpawnPosition();
        chase.transform.position = taejuSpawn;
        chase.StartChase();
    }




    private IEnumerator MoveTaejuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 태주를 트리거(TransferMap) 좌표로 이동
        chase.transform.position = this.transform.position;

        // 콜라이더가 켜져 있다면 OnTriggerEnter2D를 강제 호출하여
        Collider2D taejuCol = chase.GetComponent<Collider2D>();
        if (taejuCol != null)
        {
            // Player처럼 똑같이 포탈에 진입
            this.OnTriggerEnter2D(taejuCol); // **태주 이동 트리거**
        }
    }
}
