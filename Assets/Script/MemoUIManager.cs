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

    // Inspector에서 트리거 지정
    public MemoTrigger memoTrigger = new MemoTrigger() { dialogueCount = 5, sentenceIndex = 2 };

    private int currentPage = 0;
    private bool memoShown = false;

    private int pausedNextSentenceIndex = -1;

    // 상태머신(대기/메모 등)
    private enum Phase { None, AwaitingSpaceAtDialogueEnd, ShowingMemo }
    private Phase memoPhase = Phase.None;

    void Start()
    {
        // 모든 메모 페이지 비활성화
        foreach (var page in memoPages)
        {
            if (page != null)
                page.SetActive(false);
        }

        // OnSentenceFinished 구독
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

        // 트리거 조건 충족
        if (currentDialogueCount == memoTrigger.dialogueCount && sentenceIndex == memoTrigger.sentenceIndex)
        {
            memoShown = true;

            pausedNextSentenceIndex = sentenceIndex + 1;

            // 대화 잠시 멈춤, 스페이스 대기 상태 진입
            DialogueManager.instance.PauseDialogue();
            memoPhase = Phase.AwaitingSpaceAtDialogueEnd;
        }
    }

    void Update()
    {
        // 1. 대사 끝 후 스페이스 받고 메모로 진입
        if (memoPhase == Phase.AwaitingSpaceAtDialogueEnd)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DialogueManager.instance.HideDialogueUI();
                currentPage = 0;
                ShowCurrentPage();
                memoPhase = Phase.ShowingMemo;
            }
        }

        // 2. 메모 단계: 스페이스로 메모 넘기기
        else if (memoPhase == Phase.ShowingMemo)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HideCurrentPage();
                currentPage++;

                // 모든 메모 페이지 다 넘기면 → 대화 재개
                if (currentPage >= memoPages.Count)
                {
                    memoPhase = Phase.None;
                    DialogueManager.instance.ShowDialogueUI();

                    // 다음 문장부터 대화 이어짐
                    if (pausedNextSentenceIndex >= 0)
                    {
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
