using UnityEngine;

public class DialogueContinueTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("npc")) // npc 태그 꼭 설정!
        {
            hasTriggered = true;
            DialogueManager.instance?.ContinueDialogue(); // 대사 재개
        }
    }
}
