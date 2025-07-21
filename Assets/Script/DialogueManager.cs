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
            hasShownItemPanel = false; // 최초 1회 초기화
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
    private string playerName = "";

    private bool countUpOnFinish = true;

    // 아이템 패널 변수
    public GameObject itemPanel;
    private bool shouldHideItemPanelNext = false;
    private bool hasShownItemPanel = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Day2")
        {
            hasShownItemPanel = false;
        }
        else
        {
            // 필요 시 다른 씬에 맞춰 초기화 처리 가능
        }

        if (itemPanel != null)
            itemPanel.SetActive(false);
    }

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

        if (talking) return;
        countUpOnFinish = shouldCount;

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

        /*GameObject npcObj = GameObject.FindWithTag("npc");
        if (npcObj != null)
        {
            Animator npcAnimator = npcObj.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.SetFloat("DirX", 0);
                npcAnimator.SetFloat("DirY", -1);
            }
        }*/

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
            if (DialogueProgressManager.instance == null)
            {
                Debug.LogWarning("DialogueProgressManager가 존재하지 않습니다.");
            }
            else
            {
                DialogueProgressManager.instance.AddDialogueCount();
                Debug.Log("대화 완료! 현재 대화 수: " + DialogueProgressManager.instance.dialogueCount);
            }            
        }

        countUpOnFinish = true;

        if (SceneManager.GetActiveScene().name == "Day2")
        {
            GameObject npcObj = GameObject.FindWithTag("npc");
            if (npcObj != null)
            {
                NPCPathMover mover = npcObj.GetComponent<NPCPathMover>();
                if (mover != null)
                {
                    mover.StartPath();
                }
            }
        }

        if (SceneManager.GetActiveScene().name == "Day4"
        && DialogueProgressManager.instance.dialogueCount == 2)
        {
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("npc");
            foreach (GameObject npc in npcs)
            {
                NPCPathMover mover = npc.GetComponent<NPCPathMover>();
                if (mover != null)
                {
                    Debug.Log("ExitDialogue()에서 StartPath() 호출: " + npc.name);
                    mover.StartPath();
                }
            }
        }
    }

    IEnumerator StartDialogueCoroutine()
    {
        if (shouldHideItemPanelNext && itemPanel != null)
        {
            itemPanel.SetActive(false);
            shouldHideItemPanelNext = false;
        }

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
                    mover.StartPath();
                    talking = false;
                }
            }
        }

        
        if (currentScene == "Day2" && sentenceIndex == 2)
        {
            GameObject target = GameObject.Find("paper"); // 오브젝트 이름
            if (target != null)
            {
                target.SetActive(false);  // 오브젝트를 사라지게 함
            }

            if (!hasShownItemPanel && itemPanel != null)
            {
                itemPanel.SetActive(true);
                shouldHideItemPanelNext = true;
                hasShownItemPanel = true;
            }
        }


        if (currentScene == "Day3" && sentenceIndex == 2)
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

    public bool HasMoreSentences()
    {
        return count < listSentences.Count;
    }

    void Update()
    {
        if (talking && keyActivated && !isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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

        SkipToNextSentence();
        ContinueDialogue();
    }
}
