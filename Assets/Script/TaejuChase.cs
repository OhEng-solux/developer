using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class TaejuChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2.5f;
    public float delaySeconds = 2f;           // 지연 시간
    public float recordInterval = 0.05f;      // 위치 기록 주기
    public float followThreshold = 0.05f;

    private Rigidbody2D rb;
    private Animator animator;
    private Queue<Vector3> recordedPositions = new Queue<Vector3>();
    private float timer;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found) player = found.transform;
        }

        // 회전 금지
        rb.freezeRotation = true;

        // 태주-플레이어 충돌 무시
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("character"),
            LayerMask.NameToLayer("character"),
            true
        );
    }

    void Update()
    {
        if (!isChasing || player == null) return;

        timer += Time.deltaTime;

        // 일정 시간 간격으로 플레이어 위치 기록
        if (timer >= recordInterval)
        {
            recordedPositions.Enqueue(player.position);
            timer = 0f;
        }

        // 오래된 위치 제거 (딜레이보다 오래된 거)
        while (recordedPositions.Count > Mathf.Round(delaySeconds / recordInterval))
        {
            recordedPositions.Dequeue();
        }
    }

    void FixedUpdate()
    {
        if (!isChasing || recordedPositions.Count == 0) return;

        Vector2 target = recordedPositions.Peek();
        Vector2 current = rb.position;
        Vector2 dir = (target - current);

        if (dir.magnitude > followThreshold)
        {
            Vector2 movement = dir.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(current + movement);
            UpdateAnimation(dir);
        }
        else
        {
            recordedPositions.Dequeue();
        }
    }

    void UpdateAnimation(Vector2 direction)
    {
        animator.SetBool("Walking", true);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetFloat("DirX", direction.x > 0 ? 1 : -1);
            animator.SetFloat("DirY", 0);
        }
        else
        {
            animator.SetFloat("DirX", 0);
            animator.SetFloat("DirY", direction.y > 0 ? 1 : -1);
        }
    }

    public void StartChase()
    {
        isChasing = true;
    }

    public void StopChase()
    {
        isChasing = false;
        animator.SetBool("Walking", false);
    }
}
