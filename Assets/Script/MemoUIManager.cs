using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MemoTrigger
{
    public int dialogueCount;    // 몇 번째 ShowDialogue에서
    public int sentenceIndex;    // 그 안의 몇 번째 메시지(0부터)
}

public class MemoUIManager : MonoBehaviour
{
    public List<GameObject> memoPages;

    // Inspector에서 트리거 지정 가능
    public MemoTrigger memoTrigger = new MemoTrigger() { dialogueCount = 5, sentenceIndex = 2 };

    private int currentPage = 0;
    private bool memoActive = false;
    private bool memoShown = false;
    private int pausedNextSentenceIndex = -1;

    void Start()
    {
        // 모든 메모 페이지 비활성화
        foreach (var page in memoPages)
        {
            if (page != null)
                page.SetActive(false);
        }

        // DialogueManager에 문장 끝 이벤트 구독
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

        // 메모 조건 만족 시
        if (currentDialogueCount == memoTrigger.dialogueCount && sentenceIndex == memoTrigger.sentenceIndex)
        {
            memoShown = true;
            memoActive = true;
            currentPage = 0;
            pausedNextSentenceIndex = sentenceIndex + 1;  // 이후 이어질 인덱스 저장

            // 대화 패널 숨기고 메모 패널 띄우기
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.HideDialogueUI();
                DialogueManager.instance.PauseDialogue();
            }

            ShowCurrentPage();
        }
    }

    void Update()
    {
        if (memoActive && Input.GetKeyDown(KeyCode.Space))
        {
            HideCurrentPage();
            currentPage++;

            if (currentPage >= memoPages.Count)
            {
                memoActive = false;

                // 메모 패널 닫기
                HideCurrentPage();

                // 대화 UI 다시 띄우고 pause된 다음 문장부터 이어줌
                if (DialogueManager.instance != null && pausedNextSentenceIndex >= 0)
                {
                    DialogueManager.instance.ShowDialogueUI();
                    DialogueManager.instance.ContinueFrom(pausedNextSentenceIndex);
                    pausedNextSentenceIndex = -1;
                }
            }
            else
            {
                ShowCurrentPage();
            }
        }
    }

    private void ShowCurrentPage()
    {
        if (currentPage < memoPages.Count && memoPages[currentPage] != null)
        {
            memoPages[currentPage].SetActive(true);
        }
        else if (currentPage < memoPages.Count)
        {
            Debug.LogWarning("메모 페이지가 null입니다!");
        }
    }

    private void HideCurrentPage()
    {
        if (currentPage < memoPages.Count && memoPages[currentPage] != null)
        {
            memoPages[currentPage].SetActive(false);
        }
    }
}
