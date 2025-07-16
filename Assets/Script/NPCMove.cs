using UnityEngine;

public class NPCMover : MonoBehaviour
{
    public Transform targetPoint;
    public float moveSpeed = 2f;
    public bool isMoving { get; private set; } = false;

    public void StartMoving()
    {
        if (targetPoint != null)
            isMoving = true;
    }

    void Update()
    {
        if (isMoving && targetPoint != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
            {
                isMoving = false;

                // NPC 도착 후 사라지기 (비활성화)
                gameObject.SetActive(false);
            }
        }
    }
}
