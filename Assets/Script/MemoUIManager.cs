using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MemoTrigger
{
    public int dialogueCount;   // �� ��° ��ȭ����(=�� ��° ShowDialogue)
    public int sentenceIndex;   // �ش� ��ȭ���� �� �� ��° ����(0���� ����)
}

public class MemoUIManager : MonoBehaviour
{
    public List<GameObject> memoPages;

    // Inspector���� ���ϰ� ���� ����
    public MemoTrigger memoTrigger = new MemoTrigger() { dialogueCount = 5, sentenceIndex = 2 };

    private int currentPage = 0;
    private bool memoActive = false;
    private bool memoShown = false;

    void Start()
    {
        // ��� �޸� ������ ��Ȱ��ȭ
        foreach (var page in memoPages)
        {
            if (page != null)
                page.SetActive(false);
        }

        // **�׽�Ʈ��** : ���� �� ù ������ �Ѻ��� (�̰ŷ� UI ����� ������ Ȯ��)
        if (memoPages.Count > 0 && memoPages[0] != null)
        {
            Debug.Log("Start���� ù �޸� ������ Ȱ��ȭ �׽�Ʈ");
            // memoPages[0].SetActive(true);  // �׽�Ʈ �� �ּ� ó���ϼ���
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

        Debug.Log($"OnSentenceFinished ȣ���: dialogueCount={currentDialogueCount}, sentenceIndex={sentenceIndex}");

        // ���� ������ �޸� ���
        if (currentDialogueCount == memoTrigger.dialogueCount && sentenceIndex == memoTrigger.sentenceIndex)
        {
            Debug.Log("���� ����! �޸� ���");
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
            Debug.Log($"ShowCurrentPage ȣ��: currentPage={currentPage}, ������Ʈ={memoPages[currentPage]}");

            if (memoPages[currentPage] != null)
                memoPages[currentPage].SetActive(true);
            else
                Debug.LogWarning("�޸� �������� null�Դϴ�!");
        }
        else
        {
            Debug.LogWarning("currentPage�� memoPages.Count���� Ů�ϴ�!");
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