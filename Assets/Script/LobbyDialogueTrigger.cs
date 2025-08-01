using UnityEngine;

public class LobbyDialogueTrigger : MonoBehaviour
{
    public Dialogue[] lobbyDialogues;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player") && ClueManager.instance.flowerBroken)
        {
            hasTriggered = true;

            StartCoroutine(PlayDialogue());
        }
    }

    private System.Collections.IEnumerator PlayDialogue()
    {
        foreach (var d in lobbyDialogues)
        {
            DialogueManager.instance.ShowDialogue(d, shouldCount: false);
            yield return new WaitUntil(() => !DialogueManager.instance.talking);
        }
    }
}