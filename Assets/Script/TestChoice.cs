using UnityEngine;
using System.Collections;

public class TestChoice : MonoBehaviour
{
    [SerializeField] private Choice[] choices;
    [SerializeField] private int[] correctAnswerIndexes;
    [SerializeField] private Dialogue preDialogue;
    [SerializeField] private Dialogue successDialogue;
    [SerializeField] private Dialogue failDialogue;
    [SerializeField] private GameObject npcObject;
    [SerializeField] private Item rewardItem;


    private NPCMover npcMover;
    private OrderManager theOrder;
    private ChoiceManager theChoice;
    private DialogueManager theDM;

    private bool hasInteracted = false;
    private bool npcMoved = false;

    void Start()
    {
        theOrder = FindFirstObjectByType<OrderManager>();
        theChoice = FindFirstObjectByType<ChoiceManager>();
        theDM = FindFirstObjectByType<DialogueManager>();
        if (npcObject != null)
            npcMover = npcObject.GetComponent<NPCMover>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player" && !hasInteracted)
        {
            StartCoroutine(InteractionFlow());
        }
    }

        IEnumerator InteractionFlow()
    {
        hasInteracted = true;
        theOrder.NotMove();

        // 플레이어 이동 금지
        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = false;

        if (preDialogue != null)
        {
            theDM.ShowDialogue(preDialogue);
            yield return new WaitUntil(() => !theDM.talking);
        }

        int correctCount = 0;
        for (int i = 0; i < choices.Length; i++)
        {
            theChoice.ShowChoice(choices[i]);
            yield return new WaitUntil(() => !theChoice.choiceIng);

            int selected = theChoice.GetResult();
            if (selected == correctAnswerIndexes[i])
                correctCount++;
        }

        if (correctCount == choices.Length)
        {
            theDM.ShowDialogue(successDialogue);
            yield return new WaitUntil(() => !theDM.talking);

            InventoryManager inv = FindFirstObjectByType<InventoryManager>();
            if (rewardItem != null && inv != null)
            {
                inv.AcquireItem(rewardItem);
            }
        }
        else
        {
            npcMoved = false;
            theDM.autoNext = false;
            theDM.OnSentenceFinished += OnSentenceFinishedHandler;
            theDM.ShowDialogue(failDialogue);
            yield return new WaitUntil(() => !theDM.talking);
            theDM.OnSentenceFinished -= OnSentenceFinishedHandler;
        }

        theOrder.Move();

        // 대화 끝났으니 이동 가능하게
        if (PlayerManager.instance != null)
            PlayerManager.instance.canMove = true;
    }


    private void OnSentenceFinishedHandler(int sentenceIndex)
    {
        if (!npcMoved && sentenceIndex == 1)
        {
            npcMoved = true;
            StartCoroutine(MoveNpcAndWaitThenContinue());
        }

        
        if (sentenceIndex == 2)
        {
            theDM.WaitAndContinue(3f);
        }
    }

    IEnumerator MoveNpcAndWaitThenContinue()
    {
        theOrder.NotMove();

        if (npcMover != null)
        {
            npcMover.StartMoving();
            while (npcMover.isMoving)
                yield return null;

            if (npcObject != null)
                npcObject.SetActive(false);
        }

        theOrder.Move();
    }
}

