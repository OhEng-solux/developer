using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private List<string> listSentences = new List<string>();
    private List<Sprite> listSprites = new List<Sprite>();
    private List<Sprite> listDialogueWindows = new List<Sprite>();

    private int count;

    public Animator animSprite;
    public Animator animDialogueWindow;

    public string typeSound;
    public string enterSound;

    private AudioManager theAudio;
    private OrderManager theOrder;

    public bool talking = false;
    private bool keyActivated = false;

    public bool autoNext = false;
    public float autoNextDelay = 2.0f;

    public delegate void SentenceFinishedHandler(int sentenceIndex);
    public event SentenceFinishedHandler OnSentenceFinished;

    private bool isPaused = false;
    private bool isWaitingForName = false;
    private string playerName = "";

    private bool countUpOnFinish = true;

    void Start()
    {
        count = 0;
        text.text = "";
        listSentences = new List<string>();
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        theAudio = FindAnyObjectByType<AudioManager>();
        theOrder = FindAnyObjectByType<OrderManager>();
        OnSentenceFinished += HandleSentenceEvents;
    }

    public void ShowDialogue(Dialogue dialogue, bool shouldCount = true)
    {
        countUpOnFinish = shouldCount;

        // 기존 내용 그대로 유지
        if (dialogue.sentences.Length != dialogue.sprites.Length ||
            dialogue.sentences.Length != dialogue.dialogueWindows.Length)
        {
            Debug.LogError("[DialogueManager] Dialogue 배열 크기가 일치하지 않습니다.");
            talking = false;
            return;
        }

        talking = true;
        theOrder.NotMove();

        listSentences.Clear();
        listSprites.Clear();
        listDialogueWindows.Clear();

        GameObject npcObj = GameObject.FindWithTag("npc");
        if (npcObj != null)
        {
            Animator npcAnimator = npcObj.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.SetFloat("DirX", 0);
                npcAnimator.SetFloat("DirY", -1);
            }
        }

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]);
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }

        animSprite.SetBool("Appear", true);
        animDialogueWindow.SetBool("Appear", true);
        count = 0;

        StopAllCoroutines();
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

        if (countUpOnFinish)
        {
            DialogueProgressManager.instance.AddDialogueCount();
            Debug.Log("대화 완료! 현재 대화 수: " + DialogueProgressManager.instance.dialogueCount);
        }

        countUpOnFinish = true;

        // 여기서 Day2이고, NPC가 있으면 걷기 시작
        if (SceneManager.GetActiveScene().name == "Day2")
        {
            GameObject npcObj = GameObject.FindWithTag("npc");
            if (npcObj != null)
            {
                NPCPathMover mover = npcObj.GetComponent<NPCPathMover>();
                if (mover != null)
                {
                    mover.StartPath();  // 대화 끝나고 걷기 시작
                }
            }
        }
    }


    IEnumerator StartDialogueCoroutine()
    {
        if (count < 0 || count >= listSentences.Count)
        {
            ExitDialogue();
            yield break;
        }

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
            else if (listSprites[count] != listSprites[count - 1])
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
        else
        {
            rendererDialogueWindow.sprite = listDialogueWindows[count];
            rendererSprite.sprite = listSprites[count];
        }

        keyActivated = false;

        string playerNameForReplace = "";
        var playerMgr = FindFirstObjectByType<PlayerManager>();
        if (playerMgr != null && !string.IsNullOrEmpty(playerMgr.characterName))
            playerNameForReplace = playerMgr.characterName;
        else
            playerNameForReplace = playerName;

        string processedLine = listSentences[count].Replace("$playerName", playerNameForReplace);
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
        OnSentenceFinished?.Invoke(count);

        if (autoNext)
        {
            yield return new WaitForSeconds(autoNextDelay);
            if (keyActivated && talking)
            {
                keyActivated = false;
                count++;
                text.text = "";
                if (count == listSentences.Count)
                {
                    ExitDialogue();
                }
                else
                {
                    StartCoroutine(StartDialogueCoroutine());
                }
            }
        }
    }

    void HandleSentenceEvents(int sentenceIndex)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 이름 입력: Day1의 문장 인덱스 1에서만
        if (currentScene == "Day1" && sentenceIndex == 3)
        {
            var namePanel = FindFirstObjectByType<NameInputManager>();
            if (namePanel != null && !PlayerManager.instance.hasEnteredName)
            {
                isPaused = true;
                SetKeyInputActive(false);
                namePanel.ShowPanel();
                return;
            }
        }

        if (currentScene == "Day2" && sentenceIndex == 0)
        {
            GameObject npcObj = GameObject.FindWithTag("npc");
            if (npcObj != null)
            {
                NPCPathMover mover = npcObj.GetComponent<NPCPathMover>();
                if (mover != null)
                {
                    mover.StartPath(); // NPC가 걸어오기 시작
                    talking = false;   // 대화 잠시 종료
                }
            }
        }

        // 기존 Day3/Day5
        if (currentScene == "Day3" && sentenceIndex == 3)
        {
            GameObject npcObj = GameObject.FindWithTag("npc");
            if (npcObj != null)
            {
                NPCPathMover mover = npcObj.GetComponent<NPCPathMover>();
                if (mover != null)
                {
                    mover.StartPath();
                    talking = false;
                }
            }
        }
    }

    void Update()
    {
        if (talking && keyActivated && !isPaused)
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

    public void TemporarilyDisableKeyInput(float seconds)
    {
        StartCoroutine(ReactivateKeyAfterDelay(seconds));
    }

    private IEnumerator ReactivateKeyAfterDelay(float seconds)
    {
        keyActivated = false;
        yield return new WaitForSeconds(seconds);
        keyActivated = true;
    }

    public void SetKeyInputActive(bool value)
    {
        keyActivated = value;
    }

    public void PauseDialogue()
    {
        keyActivated = false;
        isPaused = true;
    }

    public void ContinueDialogue()
    {
        if (count >= listSentences.Count)
        {
            ExitDialogue();
            return;
        }

        talking = true;
        isPaused = false;
        SetKeyInputActive(true);

        StopAllCoroutines();
        StartCoroutine(StartDialogueCoroutine());
    }

    public void SkipToNextSentence()
    {
        count++;
    }

    public void WaitAndContinue(float seconds)
    {
        StartCoroutine(WaitAndContinueCoroutine(seconds));
    }

    private IEnumerator WaitAndContinueCoroutine(float seconds)
    {
        PauseDialogue();
        yield return new WaitForSeconds(seconds);
        SkipToNextSentence();
        ContinueDialogue();
    }

    public void OnNameInputCompleted(string inputName)
    {
        playerName = inputName;

        var player = FindFirstObjectByType<PlayerManager>();
        if (player != null)
        {
            player.characterName = inputName;
            player.hasEnteredName = true;
        }

        SkipToNextSentence(); // 현재 문장을 넘기고
        ContinueDialogue();   // 다음 문장부터 이어서 출력
    }

}
