using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MemoTrigger
{
    public int dialogueCount;
    public int sentenceIndex;
}

public class MemoUIManager : MonoBehaviour
{
    public List<GameObject> memoPages;
    public MemoTrigger memoTrigger = new MemoTrigger() { dialogueCount = 5, sentenceIndex = 2 };

    private int currentPage = 0;
    private bool memoActive = false;
    private bool memoShown = false;
    private bool isWaitingContinue = false; // 추가

    private int lastTriggeredDialogue = -1;
    private int lastTriggeredSentence = -1;

    void Start()
    {
        foreach (var page in memoPages)
            if (page != null) page.SetActive(false);

        if (DialogueManager.instance != null)
            DialogueManager.instance.OnSentenceFinished += OnSentenceFinished;
    }

    void OnDestroy()
    {
        if (DialogueManager.instance != null)
            DialogueManager.instance.OnSentenceFinished -= OnSentenceFinished;
    }

    private void OnSentenceFinished(int sentenceIndex)
    {
        if (memoShown) return;
        if (DialogueProgressManager.instance == null) return;

        int currentDialogueCount = DialogueProgressManager.instance.dialogueCount;

        // 중복 트리거 방지
        if (currentDialogueCount == lastTriggeredDialogue && sentenceIndex == lastTriggeredSentence)
            return;

        if (currentDialogueCount == memoTrigger.dialogueCount && sentenceIndex == memoTrigger.sentenceIndex)
        {
            // 중복 방지용 저장
            lastTriggeredDialogue = currentDialogueCount;
            lastTriggeredSentence = sentenceIndex;

            memoShown = true;
            memoActive = true;
            currentPage = 0;
            ShowCurrentPage();

            DialogueManager.instance.autoNext = false;
            DialogueManager.instance.PauseDialogue();
        }
    }


    void Update()
    {
        if (memoActive && !isWaitingContinue && Input.GetKeyDown(KeyCode.Space))
        {
            HideCurrentPage();
            currentPage++;

            if (currentPage >= memoPages.Count)
            {
                memoActive = false;
                isWaitingContinue = true;
                // 한 프레임 뒤에 대화 이어주기(중복방지 및 연속입력 방지)
                StartCoroutine(ContinueAfterMemo());
            }
            else
            {
                ShowCurrentPage();
            }
        }
    }

    private IEnumerator ContinueAfterMemo()
    {
        DialogueManager.instance.SetKeyInputActive(false);
        yield return null;

        DialogueManager.instance.SkipToNextSentence();  // 추가
        DialogueManager.instance.ContinueDialogue();    // 다음 문장부터 이어가기

        isWaitingContinue = false;
    }



    private void ShowCurrentPage()
    {
        if (currentPage < memoPages.Count)
        {
            if (memoPages[currentPage] != null)
                memoPages[currentPage].SetActive(true);
        }
    }

    private void HideCurrentPage()
    {
        if (currentPage < memoPages.Count)
        {
            if (memoPages[currentPage] != null)
                memoPages[currentPage].SetActive(false);
        }
    }
}
