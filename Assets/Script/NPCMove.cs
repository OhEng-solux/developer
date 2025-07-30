using UnityEngine;

public class NPCMover : MonoBehaviour
{
    public Transform targetPoint;
    public float moveSpeed = 2f;

    public bool isMoving = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    public void StartMoving()
    {
        if (targetPoint != null)
        {
            isMoving = true;
            if (animator != null)
                animator.SetBool("Walking", true); // 이동 애니메이션 재생 위한 파라미터 설정 (예시)
        }
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

                if (animator != null)
                    animator.SetBool("Walking", false); // 이동 멈췄으니 애니메이션 멈춤
            }
        }
    }
}
