using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    public GhostMove npcScript; // Inspector에서 NPC 연결

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            npcScript.StartMoving();
            Destroy(gameObject); // 트리거는 한 번만 작동
        }
    }
}
