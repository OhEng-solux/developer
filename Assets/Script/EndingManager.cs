using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class EndingManager : MonoBehaviour
{
    public GameObject Panel;

    [SerializeField] private Dialogue dialogue;                     // 대사 데이터
    [SerializeField] private GameObject endingCanvas;               // 대사 전에 보여줄 UI (선택)
    [SerializeField] private SpriteRenderer backgroundRenderer;     // Background 오브젝트의 SpriteRenderer
    [SerializeField] private FadeManager fadeManager;

    [System.Serializable]
    public class BackgroundChangeData
    {
        public int sentenceIndex;         // 몇 번째 문장에서
        public Sprite backgroundSprite;   // 어떤 배경으로 바꿀지
    }
    [SerializeField] private List<BackgroundChangeData> backgroundChanges = new();

    private DialogueManager theDM;
    private bool hasStarted = false;

    void Start()
    {
        theDM = Object.FindAnyObjectByType<DialogueManager>();
        Panel.gameObject.SetActive(false);

        if (endingCanvas != null)
            endingCanvas.SetActive(false); // 바로 대화를 시작하므로 endingCanvas는 비활성화

        Invoke(nameof(StartEndingDialogue), 0.5f); // 자동으로 대화 시작
    }

    void StartEndingDialogue()
    {
        if (hasStarted) return;

        hasStarted = true;

        if (theDM != null && dialogue != null)
        {
            theDM.ShowDialogue(dialogue);
            StartCoroutine(WaitForDialogueEnd());
        }
    }

    private IEnumerator WaitForDialogueEnd()
    {
        int previousCount = -1;

        while (theDM.talking)
        {
            int currentCount = GetCurrentSentenceIndex();

            if (currentCount != previousCount)
            {
                previousCount = currentCount;

                // 인스펙터에서 지정한 전환 조건들 확인
                foreach (var change in backgroundChanges)
                {
                    if (currentCount == change.sentenceIndex && change.backgroundSprite != null)
                    {
                        backgroundRenderer.sprite = change.backgroundSprite;
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // 안전 대기

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Ending_Bad")
        {
            SaveManager.instance.isEnding = true;
            Panel.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Start");
        }

        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //#else
        //Application.Quit();
        //#endif
    }

    // 페이드 전환 효과
    private IEnumerator SwitchBackgroundWithFade(Sprite newSprite)
    {
        if (fadeManager != null)
        {
            fadeManager.FadeOut();
            yield return new WaitForSeconds(1.0f); // 페이드아웃 완료까지 대기

            backgroundRenderer.sprite = newSprite;

            fadeManager.FadeIn();
            yield return new WaitForSeconds(1.0f); // 페이드인 완료까지 대기
        }
        else
        {
            backgroundRenderer.sprite = newSprite; // 페이드 없이 즉시 교체
        }
    }

    // DialogueManager 내부 count 값 가져오기 (private 접근)
    public int GetCurrentSentenceIndex()
    {
        var countField = typeof(DialogueManager).GetField("count", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (countField != null)
            return (int)countField.GetValue(theDM);
        return -1;
    }
    
    public bool IsDialoguePlaying()
    {
        return theDM != null && theDM.talking;
    }
}
