using UnityEngine;
using System.Collections;

public class TestNumber : MonoBehaviour
{
    [SerializeField] private Dialogue preDialogue;
    [SerializeField] private Dialogue successDialogue;
    [SerializeField] private Dialogue failDialogue;
    [SerializeField] private GameObject dialSystemObject; // NumberSystem이 붙은 오브젝트
    [SerializeField] private int correctNumber = 2235; // 정답
    [SerializeField] private Item rewardItem;

    private DialogueManager theDM;
    private OrderManager theOrder;
    private NumberSystem theNumber;
    private InventoryManager theInventory;

    private bool hasInteracted = false;
    private bool isWaitingForZ = false;
    private bool isPuzzleStarted = false;

    void Start()
    {
        theDM = FindFirstObjectByType<DialogueManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
        theNumber = dialSystemObject.GetComponent<NumberSystem>();
        theInventory = FindFirstObjectByType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasInteracted && collision.CompareTag("Player"))
        {
            hasInteracted = true;
            StartCoroutine(TriggerPuzzleStart());
        }
    }

    IEnumerator TriggerPuzzleStart()
    {
        theOrder.NotMove();
        PlayerManager.instance.canMove = false;

        // preDialogue 출력
        if (preDialogue != null)
        {
            theDM.ShowDialogue(preDialogue);
            yield return new WaitUntil(() => !theDM.talking);
        }

        // Z키 입력 대기
        isWaitingForZ = true;
        yield return new WaitUntil(() => isPuzzleStarted);
        isWaitingForZ = false;

        // 다이얼 UI 실행
        theNumber.ShowNumber(correctNumber);
        yield return new WaitUntil(() => !theNumber.activated);

        if (theNumber.GetResult()) // 성공
        {
            theDM.ShowDialogue(successDialogue);
            rewardItem.isObtained = true;
            theInventory.UpdateSlots();
        }
        else // 실패
        {
            theDM.ShowDialogue(failDialogue);
        }

        yield return new WaitUntil(() => !theDM.talking);
        theOrder.Move();
        PlayerManager.instance.canMove = true;
    }

    void Update()
    {
        if (isWaitingForZ && Input.GetKeyDown(KeyCode.Z))
        {
            isPuzzleStarted = true;
        }   
    }
}
