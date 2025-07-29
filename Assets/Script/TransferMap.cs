using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string transferMapName; // 이동할 맵의 이름
    public Transform target;
    public PolygonCollider2D targetBound;  // 카메라 바운드 변경용

    private CameraManager theCamera;
    [SerializeField] private PlayerManager thePlayer;
    private FadeManager theFade;
    private OrderManager theOrder;
    private GameObject playerLight; // 추격 조명 오브젝트

    void Start()
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
        theCamera = FindFirstObjectByType<CameraManager>();
        theFade = FindFirstObjectByType<FadeManager>();
        theOrder = FindFirstObjectByType<OrderManager>();

        playerLight = GameObject.Find("Light");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(TransferCoroutine());
        }
    }

    IEnumerator TransferCoroutine()
    {
        theOrder.NotMove();
        theFade.FadeOut();
        yield return new WaitForSeconds(1f);

        // 위치 및 카메라 바운드 설정
        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.position.x, target.position.y, theCamera.transform.position.z);
        thePlayer.transform.position = target.position;

        // === 조명 처리 ===
        if (playerLight != null)
        {
            if (transferMapName == "Basement") // 지하실 진입 시 조명 켜기
            {
                playerLight.SetActive(true);
            }
            else // 그 외 맵은 조명 끄기
            {
                playerLight.SetActive(false);
            }
        }

        // === 추격자 재배치 처리 ===
        TaejuChase chase = FindFirstObjectByType<TaejuChase>();
        if (chase != null && chase.IsChasing())
        {
            chase.SpawnAtWithDelay(thePlayer.transform.position, 2f);
        }

        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }
}