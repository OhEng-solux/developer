using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
            animator.SetFloat("DirY", 1);
        }

        if (waypoints.Count > 0)
        {
            transform.position = new Vector3(waypoints[0].x, waypoints[0].y, transform.position.z);
        }

        if (SceneManager.GetActiveScene().name == "Day2")
        {
            StartPath(); // Day2: 시작하자마자 걷기
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

                    string currentScene = SceneManager.GetActiveScene().name;

                    // Day3: 이동 후 대사 재개
                    if (currentScene == "Day3" && DialogueManager.instance != null)
                    {
                        if (DialogueManager.instance.HasMoreSentences())
                        {
                            DialogueManager.instance.talking = true;
                            DialogueManager.instance.ContinueDialogue();
                        }

                        gameObject.SetActive(false);
                    }



                    // gameObject.SetActive(false); // 필요하다면 유지
                }
            }
        }
    }

    public void StartPath()
    {
        if (waypoints.Count > 1)
        {
            isMoving = true;
            currentIndex = 1;
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
