using UnityEngine;

public class NPCMover : MonoBehaviour
{
    public Transform targetPoint;
    public float moveSpeed = 2f;

    // public �ʵ�� ���� (���� ���� �ذ��)
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
                gameObject.SetActive(false); // ���� �� ��Ȱ��ȭ
            }
        }
    }
}
