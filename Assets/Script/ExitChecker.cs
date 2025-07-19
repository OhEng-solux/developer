using System.Collections;
using UnityEngine;

public class ExitChecker : MonoBehaviour
{
    public Dialogue warningDialogue;        // "아직 대화를 다 끝내지 못한 것 같다." 대사
    public Transform targetPosition;        // 이동시킬 위치
    public string targetMapName;            // 이동할 맵 이름
    public PolygonCollider2D targetBound;   // 이동할 맵의 Bound 영역

    private CameraManager theCamera;
    private FadeManager theFade;
    private OrderManager theOrder;
    private PlayerManager thePlayer;

    void Start()
    {
        theCamera = FindFirstObjectByType<CameraManager>();
        theFade = FindFirstObjectByType<FadeManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (DialogueProgressManager.instance.dialogueCount < 9)
            {
                if (!DialogueManager.instance.talking)
                {
                    DialogueManager.instance.ShowDialogue(warningDialogue, false); // 대화만 출력
                }
            }
            else
            {
                StartCoroutine(TransferCoroutine());
            }
        }
    }

    private IEnumerator TransferCoroutine()
    {
        theOrder.NotMove();
        theFade.FadeOut();
        yield return new WaitForSeconds(1f);

        thePlayer.currentMapName = targetMapName;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(
            targetPosition.position.x,
            targetPosition.position.y,
            theCamera.transform.position.z
        );
        thePlayer.transform.position = targetPosition.position;

        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }
}
