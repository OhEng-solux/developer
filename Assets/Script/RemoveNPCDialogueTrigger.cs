using UnityEngine;
using System.Collections;

public class RemoveNPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;         // Story_dialogue
    [SerializeField] private GameObject npcToRemove;    // taeju_origin

    private DialogueManager theDM;
    private bool hasTalked = false;

    void Start()
    {
        theDM = Object.FindAnyObjectByType<DialogueManager>();

        if (theDM == null)
        {
            Debug.LogWarning("DialogueManager를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTalked) return;

        if (collision.gameObject.name == "player" && theDM != null && !theDM.talking)
        {
            theDM.ShowDialogue(dialogue);
            hasTalked = true;
            StartCoroutine(WaitAndRemoveNPC());
        }
    }

    IEnumerator WaitAndRemoveNPC()
    {
        // 대화가 끝날 때까지 기다림
        while (theDM.talking)
            yield return null;

        // NPC 제거
        if (npcToRemove != null)
        {
            npcToRemove.SetActive(false);
        }
    }
}
