using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string transferMapName; // 이동할 맵의 이름

    public Transform target;
    public PolygonCollider2D targetBound;  // 변경

    private CameraManager theCamera;
    [SerializeField] private PlayerManager thePlayer;
    private FadeManager theFade;
    private OrderManager theOrder;

    void Start()
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
        theCamera = FindFirstObjectByType<CameraManager>();
        theFade = FindFirstObjectByType<FadeManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
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
        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound); 
        theCamera.transform.position = new Vector3(target.position.x, target.position.y, theCamera.transform.position.z);
        thePlayer.transform.position = target.position;
        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }
}
