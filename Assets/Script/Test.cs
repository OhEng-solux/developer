using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Choice[] choices; // 질문 3개

    [SerializeField]
    private Dialogue correctDialogue; // 전부 정답일 때 출력할 대사

    [SerializeField]
    private Dialogue wrongDialogue;   // 하나라도 오답일 때 출력할 대사

    // 정답 인덱스를 배열로 설정 (예: 0번이 정답이면 0, 2번이 정답이면 2 등)
    [SerializeField]
    private int[] correctAnswers; // 길이는 3

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

        // 1. 질문 3개 순서대로 선택
        for (int i = 0; i < choices.Length; i++)
        {
            theChoice.ShowChoice(choices[i]);
            yield return new WaitUntil(() => !theChoice.choiceIng);
            userAnswers[i] = theChoice.GetResult();
        }

        // 2. 정답 여부 판단
        bool allCorrect = true;
        for (int i = 0; i < choices.Length; i++)
        {
            if (userAnswers[i] != correctAnswers[i])
            {
                allCorrect = false;
                break;
            }
        }

        // 3. 대사 출력
        if (allCorrect)
        {
            theDM.ShowDialogue(correctDialogue);
        }
        else
        {
            theDM.ShowDialogue(wrongDialogue);
        }

        yield return new WaitUntil(() => !theDM.talking);

        theOrder.Move(); // 이동 허용
    }
}
