using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MemoTrigger
{
    public int dialogueCount;   // 몇 번째 대화묶음(=몇 번째 ShowDialogue)
    public int sentenceIndex;   // 해당 대화묶음 내 몇 번째 문장(0부터 시작)
}

public class MemoUIManager : MonoBehaviour
{
    public List<GameObject> memoPages;

    // Inspector에서 편하게 설정 가능
    public MemoTrigger memoTrigger = new MemoTrigger() { dialogueCount = 5, sentenceIndex = 2 };

    private int currentPage = 0;
    private bool memoActive = false;
    private bool memoShown = false;

    void Start()
    {
        // 모든 메모 페이지 비활성화
        foreach (var page in memoPages)
        {
            if (page != null)
                page.SetActive(false);
        }

        // **테스트용** : 시작 시 첫 페이지 켜보기 (이거로 UI 제대로 나오나 확인)
        if (memoPages.Count > 0 && memoPages[0] != null)
        {
            Debug.Log("Start에서 첫 메모 페이지 활성화 테스트");
            // memoPages[0].SetActive(true);  // 테스트 후 주석 처리하세요
        }

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

        Debug.Log($"OnSentenceFinished 호출됨: dialogueCount={currentDialogueCount}, sentenceIndex={sentenceIndex}");

        // 조건 맞으면 메모 띄움
        if (currentDialogueCount == memoTrigger.dialogueCount && sentenceIndex == memoTrigger.sentenceIndex)
        {
            Debug.Log("조건 충족! 메모 띄움");
            memoShown = true;
            memoActive = true;
            currentPage = 0;
            ShowCurrentPage();
            DialogueManager.instance.PauseDialogue();
        }
    }

    void Update()
    {
        if (memoActive && Input.GetKeyDown(KeyCode.Z))
        {
            HideCurrentPage();
            currentPage++;

            if (currentPage >= memoPages.Count)
            {
                memoActive = false;
                DialogueManager.instance.ContinueDialogue();
            }
            else
            {
                ShowCurrentPage();
            }
        }
    }

    private void ShowCurrentPage()
    {
        if (currentPage < memoPages.Count)
        {
            Debug.Log($"ShowCurrentPage 호출: currentPage={currentPage}, 오브젝트={memoPages[currentPage]}");

            if (memoPages[currentPage] != null)
                memoPages[currentPage].SetActive(true);
            else
                Debug.LogWarning("메모 페이지가 null입니다!");
        }
        else
        {
            Debug.LogWarning("currentPage가 memoPages.Count보다 큽니다!");
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