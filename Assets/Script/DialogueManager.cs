using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private Dialogue currentDialogueData; // 현재 대화 데이터 저장용

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
    public Text blueText; // 파란색 대화 문장용
    public Text yellowText; // 노란색 대화 문장용
    public SpriteRenderer rendererSprite;
    public SpriteRenderer rendererDialogueWindow;

    private List<string> listSentences = new List<string>();
    private List<string> listBlueSentences = new List<string>(); // 파란색 대화 문장용
    private List<string> listYellowSentences = new List<string>(); // 노란색 대화 문장용
    private List<Sprite> listSprites = new List<Sprite>();
    private List<Sprite> listDialogueWindows = new List<Sprite>();
    private List<GameObject> listNPCs = new List<GameObject>(); // Day6 전용 NPC 리스트

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

    // 지금 대화를 실행 중인 오브젝트 저장을 위한 변수
    private string currentDialogueObjectName = "";

    // ★ 프롤로그에서만 사용할 다음 문장 화살표
    public GameObject nextArrow;  // Inspector에서 연결하세요

    // ★ 대화 UI 전체 캔버스 (필요시 연결해서 대화 종료 시 끌 수 있음)
    public GameObject dialogueCanvas;

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

        // 씬 바뀔 때 화살표 숨기기 (안전하게)
        if (nextArrow != null)
            nextArrow.SetActive(false);
    }

    void Start()
    {
        count = 0;
        text.text = "";
        // Day6일 때만 blue/yellow 텍스트 초기화
        if (SceneManager.GetActiveScene().name == "Day6")
        {
            if (blueText != null) blueText.text = "";
            if (yellowText != null) yellowText.text = "";
        }
        listSentences = new List<string>();
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        theAudio = FindAnyObjectByType<AudioManager>();
        theOrder = FindAnyObjectByType<OrderManager>();
        OnSentenceFinished += HandleSentenceEvents;

        // 씬이 프롤로그일 경우 화살표는 기본 숨김
        if (SceneManager.GetActiveScene().name == "Prologue" && nextArrow != null)
            nextArrow.SetActive(false);
    }

    public void ShowDialogue(Dialogue dialogue, bool shouldCount = true)
    {

        if (talking) return;
        countUpOnFinish = shouldCount;
        currentDialogueData = dialogue; // 현재 대화 데이터 저장

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
        listBlueSentences.Clear(); // 파란색 대화 문장 초기화
        listYellowSentences.Clear(); // 노란색 대화 문장 초기화
        listSprites.Clear();
        listDialogueWindows.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]);
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }

        if (dialogue.blueSentences != null)
        {
            foreach (var line in dialogue.blueSentences)
            {
                listBlueSentences.Add(line);
            }
        }

        if (dialogue.yellowSentences != null)
        {
            foreach (var line in dialogue.yellowSentences)
            {
                listYellowSentences.Add(line);
            }
        }

        if (SceneManager.GetActiveScene().name == "Day6" && dialogue.npcObjects != null)
        {
            listNPCs.AddRange(dialogue.npcObjects);
        }

        animSprite.SetBool("Appear", true);
        animDialogueWindow.SetBool("Appear", true);
        count = 0;

        // 대화 UI 캔버스 활성화 (필요시)
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(StartDialogueCoroutine());
    }

    public void ExitDialogue()
    {
        count = 0;
        text.text = "";
        if (SceneManager.GetActiveScene().name == "Day6")
        {
            if (blueText != null) blueText.text = "";
            if (yellowText != null) yellowText.text = "";
            if (blueText != null) blueText.gameObject.SetActive(false);
            if (yellowText != null) yellowText.gameObject.SetActive(false);
        }
        listSentences.Clear();
        // listBlueSentences.Clear(); // 파란색 대화 문장 초기화
        // listYellowSentences.Clear();
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

        if (SceneManager.GetActiveScene().name == "Day6")
        {
            GameObject taejuDemon = GameObject.Find("taeju_demon");
            GameObject taejuOrigin = GameObject.Find("taeju_origin");

            if (taejuDemon != null && taejuOrigin != null)
            {
                taejuOrigin.SetActive(false);     // 기존 NPC는 숨기고
                taejuDemon.SetActive(true);       // 추격용 NPC는 등장시켜

                var chaseScript = taejuDemon.GetComponent<TaejuChase>();
                if (chaseScript != null)
                {
                    chaseScript.StartChase();  // 추격 시작 (추격 스크립트에서 직접 구현해둔 메서드)
                }

                Debug.Log("[DialogueManager] 대화 종료 후 태주 추격 시작됨.");
            }
        }

        // ★ 프롤로그 씬에서는 씬 전환 없이 캔버스만 꺼줌
        if (SceneManager.GetActiveScene().name == "Prologue")
        {
            if (nextArrow != null)
                nextArrow.SetActive(false);

            if (dialogueCanvas != null)
                dialogueCanvas.SetActive(false);

            // 애니메이터도 꺼주기 (안전하게)
            animDialogueWindow.SetBool("Appear", false);
            animSprite.SetBool("Appear", false);

            text.text = "";
        }
    }

    IEnumerator StartDialogueCoroutine()
    {
        // Day6 전용: NPC 오브젝트 등장 제어
        if (SceneManager.GetActiveScene().name == "Day6")
        {
            for (int i = 0; i < listNPCs.Count; i++)
            {
                if (listNPCs[i] != null)
                    listNPCs[i].SetActive(i == count); // 해당 대사에 맞는 NPC만 활성화
            }
        }

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

        // === 문장 선택 ===
        string currentSentence = "";
        bool useBlue = false;
        bool useYellow = false;
        bool isDay6 = SceneManager.GetActiveScene().name == "Day6";

        if (isDay6)
        {
            if (count < listYellowSentences.Count && !string.IsNullOrEmpty(listYellowSentences[count]))
            {
                currentSentence = listYellowSentences[count];
                useYellow = true;
            }
            else if (count < listBlueSentences.Count && !string.IsNullOrEmpty(listBlueSentences[count]))
            {
                currentSentence = listBlueSentences[count];
                useBlue = true;
            }
        }

        if (string.IsNullOrEmpty(currentSentence) && count < listSentences.Count)
        {
            currentSentence = listSentences[count];
        }
        else if (string.IsNullOrEmpty(currentSentence))
        {
            ExitDialogue();
            yield break;
        }

        string processedLine = currentSentence.Replace("$playerName", playerNameForReplace);

        // === 텍스트 초기화 및 표시 제어 ===
        text.text = "";
        text.gameObject.SetActive(false);

        if (isDay6)
        {
            if (blueText != null) blueText.text = "";
            if (yellowText != null) yellowText.text = "";
            if (blueText != null) blueText.gameObject.SetActive(false);
            if (yellowText != null) yellowText.gameObject.SetActive(false);

            if (useBlue && blueText != null)
                blueText.gameObject.SetActive(true);
            else if (useYellow && yellowText != null)
                yellowText.gameObject.SetActive(true);
            else
                text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(true);
        }

        // === 텍스트 타이핑 출력 ===
        for (int i = 0; i < processedLine.Length; i++)
        {
            if (isDay6)
            {
                if (useBlue && blueText != null)
                    blueText.text += processedLine[i];
                else if (useYellow && yellowText != null)
                    yellowText.text += processedLine[i];
                else
                    text.text += processedLine[i];
            }
            else
            {
                text.text += processedLine[i];
            }

            if (i % 7 == 1)
                theAudio.Play(typeSound);

            yield return new WaitForSeconds(0.01f);
        }

        keyActivated = true;

        if (SceneManager.GetActiveScene().name == "Day6")
        {
            var taejuNPC = GameObject.Find("taeju_origin"); 

            if (taejuNPC != null && currentDialogueData.npcAppearances.Length > count)
            {
                // Animator 꺼주기 (덮어쓰지 않게)
                Animator npcAnimator = taejuNPC.GetComponent<Animator>();
                if (npcAnimator != null)
                    npcAnimator.enabled = false;

                // SpriteRenderer로 직접 sprite 교체
                SpriteRenderer npcSprite = taejuNPC.GetComponent<SpriteRenderer>();
                if (npcSprite != null && currentDialogueData.npcAppearances[count] != null)
                {
                    npcSprite.sprite = currentDialogueData.npcAppearances[count];
                    Debug.Log($"[DialogueManager] NPC sprite changed to: {currentDialogueData.npcAppearances[count].name}");
                }
            }
        }

        // 프롤로그 삼각형 화살표 표시
        if (SceneManager.GetActiveScene().name == "Prologue")
        {
            if (nextArrow != null)
                nextArrow.SetActive(true);
        }

        OnSentenceFinished?.Invoke(count);

        if (autoNext)
        {
            yield return new WaitForSeconds(autoNextDelay);
            if (keyActivated && talking)
            {
                keyActivated = false;
                count++;
                text.text = "";
                // blueText.text = "";
                // yellowText.text = "";
                if (count == listSentences.Count)
                {
                    ExitDialogue();
                }
                else
                {
                    StartCoroutine(StartDialogueCoroutine());
                }
                // StartCoroutine(StartDialogueCoroutine());
            }
        }
        yield break;
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

        if (currentScene == "Day3" && currentDialogueObjectName == "Hint_Dialogue" && sentenceIndex == 0)
        {
            GameObject target = GameObject.Find("memo"); // 오브젝트 이름
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

                // ★ 프롤로그 씬에서만 삼각형 화살표 숨기기
                if (SceneManager.GetActiveScene().name == "Prologue")
                {
                    if (nextArrow != null)
                        nextArrow.SetActive(false);
                }

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

    public void SetCurrentDialogueObjectName(string name) //오브젝트 이름 저장용 메소드
    {
        currentDialogueObjectName = name;
    }
}
