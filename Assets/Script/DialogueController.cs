using UnityEngine;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    private DialogueManager theDM;

    void Awake()
    {
        theDM = FindFirstObjectByType<DialogueManager>();
    }

    // <summary>
    // 일정 시간 후 다음 문장으로 넘어감
    // </summary>
    public void WaitAndContinue(float seconds)
    {
        StartCoroutine(WaitAndContinueCoroutine(seconds));
    }

    private IEnumerator WaitAndContinueCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // �ݵ�� talking ����!
        theDM.talking = true;
        theDM.SkipToNextSentence();
        theDM.SetKeyInputActive(true);
        theDM.ContinueDialogue();
    }
}
