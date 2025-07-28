using UnityEngine;
using UnityEngine.SceneManagement;  // �� ����

public class PrologueManager : MonoBehaviour
{
    [SerializeField] public Dialogue dialogue;
    private DialogueManager theDM;
    private BoxCollider2D boxCollider;
    private bool hasTriggered = false;

    public GameObject prologueCanvas; // �����Ϳ��� ����

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
                hasTriggered = true;

                // ĵ���� ��Ȱ��ȭ
                if (prologueCanvas != null)
                    prologueCanvas.SetActive(false);

                theDM.ShowDialogue(dialogue);

                boxCollider.enabled = false;

            }
        }
    }
}
