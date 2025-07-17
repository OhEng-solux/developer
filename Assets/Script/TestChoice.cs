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

    private NPCMover npcMover;
    private OrderManager theOrder;
    private ChoiceManager theChoice;
    private DialogueManager theDM;

    private bool hasInteracted = false;
    private bool npcMoved = false;

    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        theChoice = FindObjectOfType<ChoiceManager>();
        theDM = FindObjectOfType<DialogueManager>();
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
        }
        else
        {
            npcMoved = false;
            theDM.OnSentenceFinished += OnSentenceFinishedHandler;
            theDM.ShowDialogue(failDialogue);
            yield return new WaitUntil(() => !theDM.talking);
            theDM.OnSentenceFinished -= OnSentenceFinishedHandler;
        }

        theOrder.Move();
    }

    private void OnSentenceFinishedHandler(int sentenceIndex)
    {
        // NPC 이동/사라짐은 첫 문장부터 실행되게 유지
        if (!npcMoved && sentenceIndex == 0)
        {
            npcMoved = true;
            StartCoroutine(MoveNpcAndWaitThenContinue());
        }

        // 2번째 문장(인덱스 1) 끝나면 2초 멈추고 다음 문장 진행
        if (sentenceIndex == 1)
        {
            theDM.PauseDialogue();
            StartCoroutine(PauseDialogueForSeconds(5f));
        }
    }

    IEnumerator MoveNpcAndWaitThenContinue()
    {
        theOrder.NotMove();

        if (npcMover != null && npcObject != null)
        {
            npcMover.StartMoving();
            while (npcMover.isMoving)
                yield return null;

            npcObject.SetActive(false);
        }

        theOrder.Move();
    }

    IEnumerator PauseDialogueForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        theDM.SkipToNextSentence();  // 다음 문장으로 count 증가
        theDM.SetKeyInputActive(true);
        theDM.ContinueDialogue();
    }
}
