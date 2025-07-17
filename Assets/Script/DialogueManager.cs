using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

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
        }
    }
    #endregion

    public Text text;
    public SpriteRenderer rendererSprite;
    public SpriteRenderer rendererDialogueWindow;

    private List<string> listSentences;
    private List<Sprite> listSprites;
    private List<Sprite> listDialogueWindows;

    private int count;

    public Animator animSprite;
    public Animator animDialogueWindow;

    public string typeSound;
    public string enterSound;

    private AudioManager theAudio;
    private OrderManager theOrder;

    public bool talking = false;
    private bool keyActivated = false;

    // 이벤트 선언
    public delegate void SentenceFinishedHandler(int sentenceIndex);
    public event SentenceFinishedHandler OnSentenceFinished;
    //이름 입력을 위한 변수
    private bool isWaitingForName = false;
    private string playerName = "";
    public NameInputManager nameInputManager;

    void Start()
    {
        count = 0;
        text.text = "";
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        listSentences = new List<string>();
        theAudio = FindAnyObjectByType<AudioManager>();
        theOrder = FindAnyObjectByType<OrderManager>();
<<<<<<< HEAD
=======



        //이벤트 구독
        OnSentenceFinished += HandleSentenceFinished;

>>>>>>> 36b962b14f7fe3447e8d2ad1a4dd72be535b280f
    }

    //대화 중 입력창 표시
    private void HandleSentenceFinished(int sentenceIndex)
    {
        if (sentenceIndex == 3 && !FindFirstObjectByType<PlayerManager>().hasEnteredName)
        {
            isWaitingForName = true;
            keyActivated = false;
            nameInputManager.ShowPanel();
        }
    }
    /*
    public GameObject nameInputPanel;
    public InputField nameInputField; // UI 캔버스에 붙일 위치 

   private void ShowNameInputPanel()
    {
        nameInputPanel.SetActive(true);
        nameInputField.text = "";             // 입력창 초기화 (선택)
        nameInputField.ActivateInputField();   // 키보드 포커스 주기 (선택)
    }
    private void HideNameInputPanel()
    {
        nameInputPanel.SetActive(false);
    }
    */
    public void OnNameInputCompleted(string inputName)
    {
        playerName = inputName;
        FindFirstObjectByType<PlayerManager>().hasEnteredName = true;
        isWaitingForName = false;
        count++;
        ContinueDialogue();
    }


    //--입력 관련 함수

    public void ShowDialogue(Dialogue dialogue)
    {
        talking = true;
        theOrder.NotMove();

        listSentences.Clear();
        listSprites.Clear();
        listDialogueWindows.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]);
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }

        animSprite.SetBool("Appear", true);
        animDialogueWindow.SetBool("Appear", true);
        count = 0;

        StartCoroutine(StartDialogueCoroutine());
    }


    public void ExitDialogue()
    {
        count = 0;
        text.text = "";
        listSentences.Clear();
        listSprites.Clear();
        listDialogueWindows.Clear();
        animSprite.SetBool("Appear", false);
        animDialogueWindow.SetBool("Appear", false);
        talking = false;
        theOrder.Move();
        DialogueProgressManager.instance.AddDialogueCount();
    }

    IEnumerator StartDialogueCoroutine()
    {
        if (count > 0)
        {
            if (listDialogueWindows[count] != listDialogueWindows[count - 1])
            {
                animSprite.SetBool("Change", true);
                animDialogueWindow.SetBool("Appear", false);
                yield return new WaitForSeconds(0.2f);
                rendererDialogueWindow.sprite = listDialogueWindows[count];
                rendererSprite.sprite = listSprites[count];
                animDialogueWindow.SetBool("Appear", true);
                animSprite.SetBool("Change", false);
            }
            else
            {
                if (listSprites[count] != listSprites[count - 1])
                {
                    animSprite.SetBool("Change", true);
                    yield return new WaitForSeconds(0.1f);
                    rendererSprite.sprite = listSprites[count];
                    animSprite.SetBool("Change", false);
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        else
        {
            rendererDialogueWindow.sprite = listDialogueWindows[count];
            rendererSprite.sprite = listSprites[count];
        }

        keyActivated = false;
        string processedLine = listSentences[count].Replace("$playerName",FindFirstObjectByType<PlayerManager>().characterName);//이름 대입
        text.text = "";

        for (int i = 0; i < processedLine.Length; i++)
        {
            text.text += processedLine[i];
            if (i % 7 == 1)
            {
                theAudio.Play(typeSound);
            }
            yield return new WaitForSeconds(0.01f);
        }

        keyActivated = true;

        // 한 문장 출력 끝났을 때 이벤트 호출
        OnSentenceFinished?.Invoke(count);
    }

    void Update()
    {
        if (talking && keyActivated && !isWaitingForName)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                keyActivated = false;
                count++;
                text.text = "";
                theAudio.Play(enterSound);

                if (count == listSentences.Count)
                {
                    StopAllCoroutines();
                    ExitDialogue();
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(StartDialogueCoroutine());
                }
            }
        }
    }

    public void ContinueDialogue()
    {
        if (talking && !keyActivated)
        {
            StopAllCoroutines();
            StartCoroutine(StartDialogueCoroutine());
        }
    }
}