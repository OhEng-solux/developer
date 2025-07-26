using UnityEngine;
using System.Collections;

public class TestNumber : MonoBehaviour
{
    [SerializeField] private Dialogue preDialogue;
    [SerializeField] private Dialogue successDialogue;
    [SerializeField] private Dialogue failDialogue;
    [SerializeField] private GameObject dialSystemObject; // NumberSystem이 붙은 오브젝트
    [SerializeField] private int correctNumber = 2265; // 정답 숫자
    [SerializeField] private Item rewardItem; // 성공 시 획득할 아이템

    private DialogueManager theDM;
    private OrderManager theOrder;
    private NumberSystem theNumber;
    private InventoryManager theInventory;

    // 상태 변수
    private bool isPlayerInTrigger = false; // 플레이어가 퍼즐 콜라이더에 들어와 있는가
    private bool hasInteracted = false;     // 성공 or 실패로 이미 시도한 적이 있는가
    private bool hasPlayedPreDialogue = false; // preDialogue는 한 번만 출력
    private bool isWaitingForZ = false;     // Z 키를 기다리는 상태인가 (대화 이후)
    private bool isPuzzleStarted = false;   // Z 키를 눌러 퍼즐을 시작했는가

    void Start()
    {
        theDM = FindFirstObjectByType<DialogueManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
        theNumber = dialSystemObject.GetComponent<NumberSystem>();
        theInventory = FindFirstObjectByType<InventoryManager>();
    }

    // 플레이어가 콜라이더에 들어오면 퍼즐 시도 조건 확인
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasInteracted && collision.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            Vector3 pushBack = new Vector3(-0.03f, 0f, 0f); // 왼쪽으로 살짝 밀기
            collision.transform.position += pushBack;

            StartCoroutine(TriggerPuzzleStart());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            // 퍼즐 대기 상태 초기화
            if (isWaitingForZ)
            {
                isWaitingForZ = false;
                isPuzzleStarted = false;
                PlayerManager.instance.canMove = true; // 다시 이동 가능하게
                theOrder.Move();
            }
        }
    }

    IEnumerator TriggerPuzzleStart()
    {
        // 상태 초기화
        isPuzzleStarted = false;
        isWaitingForZ = true;
        dialSystemObject.SetActive(false); // 혹시 이전 UI 남아 있을 경우 꺼주기

        // PlayerManager.instance.canMove = false;
        // theOrder.NotMove();

        // 사전 대사는 처음 한 번만 출력
        if (!hasPlayedPreDialogue && preDialogue != null)
        {
            hasPlayedPreDialogue = true;
            theDM.ShowDialogue(preDialogue);
            yield return new WaitUntil(() => !theDM.talking);
        }

        // Z 키가 눌릴 때까지 대기
        yield return new WaitUntil(() => isPuzzleStarted);
        
        // 퍼즐 시작
        theNumber.ShowNumber(correctNumber);
        yield return new WaitUntil(() => !theNumber.activated); // 퍼즐 종료 대기

        // 퍼즐 결과에 따라 처리
        if (theNumber.GetResult()) // 정답
        {
            theDM.ShowDialogue(successDialogue);
            if (rewardItem != null && theInventory != null)
            { // 아이템 획득 처리
                theInventory.AcquireItem(rewardItem); 
            }
            hasInteracted = true; // 재시도 불가
        }
        else if (!theNumber.WasCancelled()) // 오답
        {
            theDM.ShowDialogue(failDialogue);
            hasInteracted = true; // 재시도 불가
        }
        else // 취소 (ESC) → 재도전 허용
        {
            hasInteracted = false;
        }

        yield return new WaitUntil(() => !theDM.talking);
        theOrder.Move();
        PlayerManager.instance.canMove = true;
    }

    void Update()
    {
        // Z 키를 눌러 퍼즐 시작 조건 만족 시
        if (isWaitingForZ && isPlayerInTrigger && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("[Update] 퍼즐 시작 조건 만족!");
            isPuzzleStarted = true;
            isWaitingForZ = false;
            PlayerManager.instance.canMove = false;
            dialSystemObject.SetActive(true);
        }
    }
}