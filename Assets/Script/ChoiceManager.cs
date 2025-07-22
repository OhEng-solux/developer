using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager instance;

    #region Singleton
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        if (go != null) go.SetActive(false);
        if (anim != null) anim.SetBool("Appear", false);
    }
    #endregion

    private AudioManager theAudio;
    private OrderManager theOrder;

    private string question;
    private List<string> answerList;

    public GameObject go;
    public Text question_Text;
    public Text[] answer_Text;       // 각 선택지 텍스트
    public GameObject[] answer_Panel; // 각 선택지 패널
    public RectTransform selectorImage; // "▶" 등 하나만 할당

    public Animator anim;

    public string keySound;
    public string enterSound;

    public bool choiceIng = false;
    private bool keyInput = false;

    private int lastAnswerIndex = -1;
    private int result;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    void Start()
    {
        theAudio = FindFirstObjectByType<AudioManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
        answerList = new List<string>();

        for (int i = 0; i < answer_Text.Length; i++)
        {
            answer_Text[i].text = "";
            answer_Panel[i].SetActive(false);
        }

        question_Text.text = "";
        if (selectorImage != null) selectorImage.gameObject.SetActive(true);
    }

    public void ShowChoice(Choice _choice)
    {
        choiceIng = true;
        theOrder.NotMove();

        go.SetActive(true);
        result = 0;
        question = _choice.question;

        answerList.Clear();
        int maxAnswers = Mathf.Min(answer_Text.Length, answer_Panel.Length, _choice.answers.Length);
        for (int i = 0; i < maxAnswers; i++)
        {
            answerList.Add(_choice.answers[i]);
            answer_Panel[i].SetActive(true);
        }
        lastAnswerIndex = maxAnswers - 1;

        anim.SetBool("Appear", true);
        Selection(); // 선택지 처음에 selectorImage 위치 바로 맞추기
        StartCoroutine(ChoiceCoroutine());
    }

    public int GetResult()
    {
        return result;
    }

    public void ExitChoice()
    {
        question_Text.text = "";
        if (lastAnswerIndex >= 0)
        {
            for (int i = 0; i <= lastAnswerIndex; i++)
            {
                answer_Text[i].text = "";
                answer_Panel[i].SetActive(false);
            }
        }

        answerList.Clear();
        anim.SetBool("Appear", false);
        go.SetActive(false);
        choiceIng = false;
        if (selectorImage != null) selectorImage.gameObject.SetActive(false); // 꺼주기
        theOrder.Move();
    }

    IEnumerator ChoiceCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(TypingQuestion());

        if (answerList.Count > 0) StartCoroutine(TypingAnswer_0());
        if (answerList.Count > 1) StartCoroutine(TypingAnswer_1());
        if (answerList.Count > 2) StartCoroutine(TypingAnswer_2());
        if (answerList.Count > 3) StartCoroutine(TypingAnswer_3());

        yield return new WaitForSeconds(0.5f);
        keyInput = true;
    }

    IEnumerator TypingQuestion()
    {
        question_Text.text = "";
        for (int i = 0; i < question.Length; i++)
        {
            question_Text.text += question[i];
            yield return waitTime;
        }
    }

    IEnumerator TypingAnswer_0()
    {
        answer_Text[0].text = "";
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < answerList[0].Length; i++)
        {
            answer_Text[0].text += answerList[0][i];
            yield return waitTime;
        }
    }

    IEnumerator TypingAnswer_1()
    {
        answer_Text[1].text = "";
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < answerList[1].Length; i++)
        {
            answer_Text[1].text += answerList[1][i];
            yield return waitTime;
        }
    }

    IEnumerator TypingAnswer_2()
    {
        answer_Text[2].text = "";
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < answerList[2].Length; i++)
        {
            answer_Text[2].text += answerList[2][i];
            yield return waitTime;
        }
    }

    IEnumerator TypingAnswer_3()
    {
        answer_Text[3].text = "";
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < answerList[3].Length; i++)
        {
            answer_Text[3].text += answerList[3][i];
            yield return waitTime;
        }
    }

    void Update()
    {
        if (!keyInput) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            theAudio.Play(keySound);
            result = (result > 0) ? result - 1 : lastAnswerIndex;
            Selection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            theAudio.Play(keySound);
            result = (result < lastAnswerIndex) ? result + 1 : 0;
            Selection();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            theAudio.Play(enterSound);
            keyInput = false;
            ExitChoice();
        }
    }

    public void Selection()
    {
        if (selectorImage == null)
        {
            Debug.LogWarning("SelectorImage가 연결되지 않았습니다!");
            return;
        }

        Debug.Log($"Selector 이동: {result}, 기존 부모: {selectorImage.parent.name}");

        selectorImage.SetParent(answer_Panel[result].transform, false);

        Debug.Log($"Selector 새 부모: {selectorImage.parent.name}");

        selectorImage.localScale = Vector3.one;
        selectorImage.localRotation = Quaternion.identity;

        // var baseSize = new Vector2(7.4f, 8.4f);
        // selectorImage.sizeDelta = baseSize;

        // selectorImage.anchoredPosition = new Vector2(-25f, 0f);
    }




}
