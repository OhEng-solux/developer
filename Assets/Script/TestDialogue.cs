using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class TestDialogue : MonoBehaviour
{
    [SerializeField] public Dialogue dialogue;
    private DialogueManager theDM;
    private BoxCollider2D boxCollider;
    private bool hasTriggered = false; //  추가

    void Start()
    {
        theDM = Object.FindAnyObjectByType<DialogueManager>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player" && !hasTriggered)
        {
            if (!theDM.talking)
            {
                hasTriggered = true; // 한 번만 트리거 되게 함
                theDM.SetCurrentDialogueObjectName(this.gameObject.name); // 대화 실행 중인 오브젝트 이름 전달
                theDM.ShowDialogue(dialogue);
                boxCollider.enabled = false;
            }
        }
    }
}
