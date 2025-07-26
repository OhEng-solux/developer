using UnityEngine;
using System.Collections;

public class ChaseTriggerManager : MonoBehaviour
{
    public Dialogue[] chaseDialogues; // Inspector에서 demon_1~3 순으로 등록
    private int currentDialogueIndex = 0;

    private bool isChasing = false;
    private bool dialogueStarted = false;

    public GameObject taejuNPC; // 태주 NPC
    public float chaseDelay = 1.0f;
    public float fadeInDuration = 1.0f;

    void Update()
    {
        if (!isChasing && dialogueStarted && DialogueManager.instance != null && !DialogueManager.instance.talking)
        {
            if (HasMoreDialogue())
            {
                ShowNextDialogue();
            }
            else
            {
                isChasing = true;
                Invoke(nameof(StartChase), chaseDelay);
            }
        }
    }

    public void StartChaseDialogue()
    {
        StartCoroutine(ShowTaejuAndDialogue());
    }

    private IEnumerator ShowTaejuAndDialogue()
    {
        if (taejuNPC != null)
        {
            taejuNPC.SetActive(true);

            SpriteRenderer sr = taejuNPC.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 알파값 0으로 초기화
                Color color = sr.color;
                color.a = 0f;
                sr.color = color;

                float timer = 0f;
                while (timer < fadeInDuration)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
                    sr.color = new Color(color.r, color.g, color.b, alpha);
                    yield return null;
                }

                sr.color = new Color(color.r, color.g, color.b, 1f);
            }
        }

        yield return new WaitForSeconds(0.5f); // 페이드인 후 약간 대기

        dialogueStarted = true;
        if (DialogueManager.instance != null && currentDialogueIndex < chaseDialogues.Length)
        {
            DialogueManager.instance.ShowDialogue(chaseDialogues[currentDialogueIndex]);
            currentDialogueIndex++;
        }
    }

    private void ShowNextDialogue()
    {
        if (currentDialogueIndex < chaseDialogues.Length)
        {
            DialogueManager.instance.ShowDialogue(chaseDialogues[currentDialogueIndex]);
            currentDialogueIndex++;
        }
    }

    public bool HasMoreDialogue()
    {
        return currentDialogueIndex < chaseDialogues.Length;
    }

    public void StartChase()
    {
        Debug.Log("추격 시작!");
        // 여기서 태주가 움직이게 하는 코드 넣기
    }
}
