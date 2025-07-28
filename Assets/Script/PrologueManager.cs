using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 관련

public class PrlogueManager : MonoBehaviour
{
    [SerializeField] public Dialogue dialogue;
    private DialogueManager theDM;
    private BoxCollider2D boxCollider;
    private bool hasTriggered = false;

    public GameObject prologueCanvas; // 에디터에서 연결

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

                // 캔버스 비활성화
                if (prologueCanvas != null)
                    prologueCanvas.SetActive(false);

                theDM.ShowDialogue(dialogue);

                boxCollider.enabled = false;

            }
        }
    }
}
