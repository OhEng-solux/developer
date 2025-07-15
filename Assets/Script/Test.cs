using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Choice[] choices; // ���� 3��

    [SerializeField]
    private Dialogue correctDialogue; // ���� ������ �� ����� ���

    [SerializeField]
    private Dialogue wrongDialogue;   // �ϳ��� ������ �� ����� ���

    // ���� �ε����� �迭�� ���� (��: 0���� �����̸� 0, 2���� �����̸� 2 ��)
    [SerializeField]
    private int[] correctAnswers; // ���̴� 3

    private OrderManager theOrder;
    private ChoiceManager theChoice;
    private DialogueManager theDM;

    public bool flag;

    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        theChoice = FindObjectOfType<ChoiceManager>();
        theDM = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag && collision.gameObject.name == "player")
        {
            StartCoroutine(MultiQuestionCoroutine());
        }
    }

    IEnumerator MultiQuestionCoroutine()
    {
        flag = true;
        theOrder.NotMove();

        int[] userAnswers = new int[choices.Length];

        // 1. ���� 3�� ������� ����
        for (int i = 0; i < choices.Length; i++)
        {
            theChoice.ShowChoice(choices[i]);
            yield return new WaitUntil(() => !theChoice.choiceIng);
            userAnswers[i] = theChoice.GetResult();
        }

        // 2. ���� ���� �Ǵ�
        bool allCorrect = true;
        for (int i = 0; i < choices.Length; i++)
        {
            if (userAnswers[i] != correctAnswers[i])
            {
                allCorrect = false;
                break;
            }
        }

        // 3. ��� ���
        if (allCorrect)
        {
            theDM.ShowDialogue(correctDialogue);
        }
        else
        {
            theDM.ShowDialogue(wrongDialogue);
        }

        yield return new WaitUntil(() => !theDM.talking);

        theOrder.Move(); // �̵� ���
    }
}
