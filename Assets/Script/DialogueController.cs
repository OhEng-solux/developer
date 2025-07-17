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
    /// ���� �ð� ���� �� ���� �������� �ڵ� ����
    /// </summary>
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
