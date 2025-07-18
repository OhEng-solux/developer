using UnityEngine;
using System.Collections.Generic;

public class NPCPathMover : MonoBehaviour
{
    public List<Vector2> waypoints;
    public float moveSpeed = 2f;

    private int currentIndex = 0;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetFloat("DirX", 0);
            animator.SetFloat("DirY", -1);
        }

        if (waypoints.Count > 0)
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

            transform.position = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);

            if (animator != null)
            {
                animator.SetFloat("DirX", direction.x);
                animator.SetFloat("DirY", direction.y);
                animator.SetBool("Walking", true);
            }

            if (Vector2.Distance(current, target) < 0.01f)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Count)
                {
                    isMoving = false;
                    if (animator != null)
                        animator.SetBool("Walking", false);

                    // 이동 후 대사 재개
                    if (DialogueManager.instance != null)
                    {
                        DialogueManager.instance.talking = true;
                        DialogueManager.instance.ContinueDialogue();
                    }

                    gameObject.SetActive(false);
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
            transform.position = new Vector3(waypoints[0].x, waypoints[0].y, transform.position.z);
        }
    }

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
