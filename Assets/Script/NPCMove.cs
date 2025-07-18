using UnityEngine;

public class NPCMover : MonoBehaviour
{
    public Transform targetPoint;
    public float moveSpeed = 2f;

    // public 필드로 변경 (접근 문제 해결용)
    public bool isMoving = false;

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
                gameObject.SetActive(false); // 도착 후 비활성화
            }
        }
    }
}
