using UnityEngine;
using System.Collections.Generic;

public class NPCPathMover : MonoBehaviour
{
    public List<Vector2> waypoints; // 이동할 지점들
    public float moveSpeed = 2f;
    private int currentIndex = 0;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isMoving && currentIndex < waypoints.Count)
        {
            Vector2 target = waypoints[currentIndex];
            Vector2 current = transform.position;
            Vector2 direction = (target - current).normalized;

            transform.position = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);

            if (animator != null)
            {
                animator.SetFloat("DirX", direction.x);
                animator.SetFloat("DirY", direction.y);
                animator.SetBool("Walking", true);
            }

            if (Vector2.Distance(current, target) < 0.05f)
            {
                currentIndex++;

                if (currentIndex >= waypoints.Count)
                {
                    isMoving = false;
                    if (animator != null) animator.SetBool("Walking", false);

                    // 대사 이어서 진행
                    DialogueManager.instance?.ContinueDialogue();
                }
            }
        }
    }

    public void StartPath()
    {
        if (waypoints.Count > 0)
        {
            isMoving = true;
            currentIndex = 0;
        }
    }
}
