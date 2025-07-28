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
    private bool isWaitingContinue = false;

    private int lastTriggeredDialogue = -1;
    private int lastTriggeredSentence = -1;

    void Start()
    {
        // 메모 페이지 모두 비활성화
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
        if (DialogueManager.instance == null) return;

        int currentDialogueCount = DialogueProgressManager.instance.dialogueCount;

        // 중복 트리거 방지
        if (currentDialogueCount == lastTriggeredDialogue && sentenceIndex == lastTriggeredSentence)
            return;

        if (currentDialogueCount == memoTrigger.dialogueCount && sentenceIndex == memoTrigger.sentenceIndex)
        {
            lastTriggeredDialogue = currentDialogueCount;
            lastTriggeredSentence = sentenceIndex;

            memoShown = true;
            memoActive = true;
            currentPage = 0;

            // 메모가 뜰 때 대화 UI 숨김
            DialogueManager.instance.autoNext = false;
            DialogueManager.instance.PauseDialogue();
            DialogueManager.instance.HideDialogueUI();

            ShowCurrentPage();
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

                // 메모 끝난 뒤 다음 프레임에 대화 이어가기
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

        // 3번째 대화부터 진행하도록 설정 (인덱스 기준이라면 2 또는 3으로 조정할 것)
        DialogueProgressManager.instance.dialogueCount = 2;

        DialogueManager.instance.SkipToNextSentence();
        DialogueManager.instance.ContinueDialogue();

        DialogueManager.instance.ShowDialogueUI();

        isWaitingContinue = false;
    }


    private void ShowCurrentPage()
    {
        if (currentPage < memoPages.Count && memoPages[currentPage] != null)
            memoPages[currentPage].SetActive(true);
    }

    private void HideCurrentPage()
    {
        if (currentPage < memoPages.Count && memoPages[currentPage] != null)
            memoPages[currentPage].SetActive(false);
    }
}
