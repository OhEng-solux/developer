using UnityEngine;
using System.Collections;

public class TestDialogue : MonoBehaviour
{
    [SerializeField]
    public Dialogue dialogue; // 대화 데이터 (ScriptableObject)

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
        // 이미 대화한 적 있으면 무시
        if (hasTalked) return;

        // 플레이어와 충돌했고, 대화가 가능한 상태일 때
        if (collision.gameObject.name == "player" && theDM != null && !theDM.talking)
        {
            theDM.ShowDialogue(dialogue);
            hasTalked = true;
            StartCoroutine(DisableAfterDialogue());
        }
    }

    IEnumerator DisableAfterDialogue()
    {
        // 1. 대화가 끝날 때까지 기다림
        while (theDM.talking)
            yield return null;

        // 2. 플레이어가 트리거 밖으로 나갈 때까지 기다림
        Collider2D playerCollider = GameObject.Find("player")?.GetComponent<Collider2D>();
        Collider2D myCollider = GetComponent<Collider2D>();

        // 둘 다 null이 아니고, 아직 충돌 중이면 대기
        while (playerCollider != null && myCollider != null && myCollider.IsTouching(playerCollider))
        {
            yield return null;
        }

        // 3. 안전하게 비활성화
        //gameObject.SetActive(false);
    }
}
