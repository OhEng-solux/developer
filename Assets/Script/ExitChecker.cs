using System.Collections;
using UnityEngine;

public class ExitChecker : MonoBehaviour
{
    public Dialogue warningDialogue;        // "아직 대화를 다 끝내지 못한 것 같다." 대사
    public Transform targetPosition;        // 이동시킬 위치
    public string targetMapName;            // 이동할 맵 이름
    public PolygonCollider2D targetBound;   // 이동할 맵의 Bound 영역

    [Tooltip("필요한 최소 대화 진행 수 (기본 9)")]
    public int requiredDialogueCount = 9;   // 기본값 9

    private CameraManager theCamera;
    private FadeManager theFade;
    private OrderManager theOrder;
    private PlayerManager thePlayer;

    private float talkCooldown = 1f; // 재대화 가능 시간 간격 (초)
    private float lastTalkTime = -10f; // 마지막 대화 시간 초기화

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
            float currentTime = Time.time;

            if (DialogueProgressManager.instance.dialogueCount < requiredDialogueCount)
            {
                if (!DialogueManager.instance.talking && currentTime - lastTalkTime > talkCooldown)
                {
                    lastTalkTime = currentTime;
                    DialogueManager.instance.ShowDialogue(warningDialogue, false); // 대화만 출력
                }
            }
            else
            {
                // 쿨타임 없이 바로 이동 처리
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
