using UnityEngine;

public class DialogueContinueTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
{
    Debug.Log($"[Trigger] 충돌 감지됨: {collision.gameObject.name}");

    if (!hasTriggered && collision.CompareTag("npc"))
    {
        hasTriggered = true;
        Debug.Log("태그 일치! ContinueDialogue 호출");
        DialogueManager.instance.talking = true; 
        // ContinueDialogue()를 호출하기 전에 다시 talking을 true로 설정해줘야 실행 가능
        DialogueManager.instance?.ContinueDialogue();
    }
}
}
