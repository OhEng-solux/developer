using UnityEngine;

public class DayStart : MonoBehaviour
{
    public Dialogue dialogue;               // 대화 데이터
    private DialogueManager theDM;
    private BoxCollider2D boxCollider;

    void Start()
    {
        theDM = FindAnyObjectByType<DialogueManager>();
        boxCollider = GetComponent<BoxCollider2D>();

        // 시작할 때 콜라이더 끄기 (원하면 켤 수도 있음)
        boxCollider.enabled = false;
    }

    public void TriggerDialogue()
    {
        if (!theDM.talking)
        {
            theDM.ShowDialogue(dialogue);
            boxCollider.enabled = false;    // 대화 시작 시 콜라이더 끄기
        }
    }

    // 대화가 끝난 후 다시 콜라이더 켜는 함수
    public void EnableCollider()
    {
        if (boxCollider != null)
            boxCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            if (!theDM.talking)
            {
                theDM.ShowDialogue(dialogue);
                boxCollider.enabled = false;
            }
        }
    }
}
