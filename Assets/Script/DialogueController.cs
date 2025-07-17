using UnityEngine;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    private DialogueManager theDM;

    void Awake()
    {
        theDM = FindObjectOfType<DialogueManager>();
    }

    /// <summary>
    /// 일정 시간 멈춘 뒤 다음 문장으로 자동 진행
    /// </summary>
    public void WaitAndContinue(float seconds)
    {
        StartCoroutine(WaitAndContinueCoroutine(seconds));
    }

    private IEnumerator WaitAndContinueCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // 반드시 talking 복구!
        theDM.talking = true;
        theDM.SkipToNextSentence();
        theDM.SetKeyInputActive(true);
        theDM.ContinueDialogue();
    }
}
