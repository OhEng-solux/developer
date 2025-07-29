using System.Collections;
using UnityEngine;

public class TransferMap : MonoBehaviour
{
    public string transferMapName; // 이동할 맵의 이름
    public Transform target;
    public PolygonCollider2D targetBound;

    private CameraManager theCamera;
    private PlayerManager thePlayer;
    private FadeManager theFade;
    private OrderManager theOrder;
    private GameObject playerLight;

    void Awake()
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
            Debug.Log("[TransferMap] 플레이어가 트리거 진입함, 코루틴 시작"); 
            StartCoroutine(TransferCoroutine());
        }
    }

    IEnumerator TransferCoroutine()
    {
        Debug.Log($"[Light] transferMapName: {transferMapName} → {(transferMapName == "Basement" ? "조명 켜짐" : "조명 꺼짐")}");

        theOrder.NotMove();
        theFade.FadeOut();
        yield return new WaitForSeconds(1f);

        // 위치 및 카메라 바운드 설정
        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.position.x, target.position.y, theCamera.transform.position.z);
        thePlayer.transform.position = target.position;

        // 조명 처리
        if (playerLight != null)
        {
            bool isBasement = transferMapName.Trim().ToLower() == "basement";
            playerLight.SetActive(isBasement);

            Debug.Log($"[Light] transferMapName: {transferMapName} → 조명 {(isBasement ? "켜짐" : "꺼짐")}");
        }


        // 추격자 재배치
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
