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
        // NPC �̵�/������� ù ������� ����ǰ� ����
        if (!npcMoved && sentenceIndex == 0)
        {
            npcMoved = true;
            StartCoroutine(MoveNpcAndWaitThenContinue());
        }

        // 2��° ����(�ε��� 1) ������ 2�� ���߰� ���� ���� ����
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

        theDM.SkipToNextSentence();  // ���� �������� count ����
        theDM.SetKeyInputActive(true);
        theDM.ContinueDialogue();
    }
}
