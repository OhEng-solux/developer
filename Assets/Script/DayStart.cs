using UnityEngine;

public class DayStart : MonoBehaviour
{
    public Dialogue dialogue;               // ��ȭ ������
    private DialogueManager theDM;
    private BoxCollider2D boxCollider;

    void Start()
    {
        theDM = FindAnyObjectByType<DialogueManager>();
        boxCollider = GetComponent<BoxCollider2D>();

        // ������ �� �ݶ��̴� ���� (���ϸ� �� ���� ����)
        boxCollider.enabled = false;
    }

    public void TriggerDialogue()
    {
        if (!theDM.talking)
        {
            theDM.ShowDialogue(dialogue);
            boxCollider.enabled = false;    // ��ȭ ���� �� �ݶ��̴� ����
        }
    }

    // ��ȭ�� ���� �� �ٽ� �ݶ��̴� �Ѵ� �Լ�
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
