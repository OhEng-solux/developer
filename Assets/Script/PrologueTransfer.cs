using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PrologueTransfer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private DialogueManager theDM;
    private DialogueProgressManager dpm;
    private FadeManager theFade;
    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
        dpm = FindObjectOfType<DialogueProgressManager>();
        theFade = FindObjectOfType<FadeManager>();
        if (theDM == null)
        {
            Debug.LogError("DialogueManager가 씬에 없습니다!");
            return;
        }

        // 대화 종료를 감지하기 위해 코루틴 시작
        StartCoroutine(WaitForDialogueEnd());
    }

    private IEnumerator WaitForDialogueEnd()
    {
        // 대화가 끝날 때까지 대기
        while (dpm.dialogueCount == 0)
        {
            yield return null;  // 매 프레임 체크
        }
        theFade.FadeOut();
        // 대화 종료 후 바로 씬 전환
        SceneManager.LoadScene("Day1");
    }
}
