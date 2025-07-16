using UnityEngine;
using System.Collections.Generic;

public class NPCPathMover : MonoBehaviour
{
    public List<Vector2> waypoints; // 이동할 지점들 (월드 좌표)
    public float moveSpeed = 2f;

    private int currentIndex = 0;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("DirX", 0);
            anim.SetFloat("DirY", 1); // 위쪽을 기본 방향으로 설정(뒤돌아보도록)
        }

        // 경로가 있으면 NPC의 위치를 첫 경로에 맞춤
        if (waypoints.Count >= 0)
        {
            transform.position = new Vector3(waypoints[0].x, waypoints[0].y, transform.position.z);
        }
    }

    void Update()
    {
        if (isMoving && currentIndex < waypoints.Count)
        {
            Vector2 target = waypoints[currentIndex];
            Vector2 current = new Vector2(transform.position.x, transform.position.y);
            Vector2 direction = (target - current).normalized;

            // 이동
            transform.position = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);

            // 애니메이션 처리
            if (animator != null)
            {
                animator.SetFloat("DirX", direction.x);
                animator.SetFloat("DirY", direction.y);
                animator.SetBool("Walking", true);
                Debug.Log($"[Animator] Walking = true, Dir = ({direction.x}, {direction.y})");
            }

            // 거리 체크 및 다음 웨이포인트로 이동
            if (Vector2.Distance(current, target) < 0.01f)
            {
                currentIndex++;

                // 마지막 도착 후 처리
                if (currentIndex >= waypoints.Count)
                {
                    isMoving = false;

                    if (animator != null)
                        animator.SetBool("Walking", false);

                    DialogueManager.instance?.ContinueDialogue();
                    gameObject.SetActive(false); // 이동 완료 후 비활성화
                }
            }
        }
    }

    // 외부에서 호출하는 이동 시작 함수
    public void StartPath()
    {
        if (waypoints.Count > 0)
        {
            isMoving = true;
            currentIndex = 0;

            // 처음 위치 보정 (혹시 Start에서 이동 못 했을 경우)
            transform.position = new Vector3(waypoints[0].x, waypoints[0].y, transform.position.z);
        }
    }

    // 씬 뷰에서 경로 확인용 Gizmo
    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Count < 2)
            return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }
    }
}
