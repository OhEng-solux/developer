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

    void Start()
    {
        count = 0;
        text.text = "";
        theAudio = FindAnyObjectByType<AudioManager>();
        theOrder = FindAnyObjectByType<OrderManager>();
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        if (dialogue.sentences.Length != dialogue.sprites.Length ||
            dialogue.sentences.Length != dialogue.dialogueWindows.Length)
        {
            Debug.LogError($"[DialogueManager] Dialogue 배열 크기가 일치하지 않습니다. ({dialogue.sentences.Length}, {dialogue.sprites.Length}, {dialogue.dialogueWindows.Length})");
            talking = false;
            return;
        }

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
    }

    IEnumerator StartDialogueCoroutine()
    {
        if (count < 0 ||
            count >= listSentences.Count ||
            count >= listSprites.Count ||
            count >= listDialogueWindows.Count)
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
        text.text = "";

        for (int i = 0; i < listSentences[count].Length; i++)
        {
            text.text += listSentences[count][i];
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

    void Update()
    {
        if (talking && keyActivated)
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
    }

    public void ContinueDialogue()
    {
        if (count >= listSentences.Count)
        {
            ExitDialogue();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(StartDialogueCoroutine());
    }

    // 추가: 다음 문장으로 건너뛰기용 공개 메서드
    public void SkipToNextSentence()
    {
        count++;
    }
}
