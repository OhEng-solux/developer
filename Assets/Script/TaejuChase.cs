using UnityEngine;

public class TaejuChase : MonoBehaviour
{
    public Transform player;           // 추격 대상 (플레이어)
    public float moveSpeed = 2.5f;     // 추격 속도 (플레이어랑 동일하게)
    public float stoppingDistance = 0.1f; // 너무 가까워지면 멈춤

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movement;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("Player를 찾을 수 없습니다. Tag가 'Player'인지 확인하세요.");
    }

    void Update()
    {
        if (!isChasing || player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance > stoppingDistance)
        {
            movement = direction;
            animator.SetBool("Walking", true);
            animator.SetFloat("DirX", direction.x);
            animator.SetFloat("DirY", direction.y);
        }
        else
        {
            movement = Vector2.zero;
            animator.SetBool("Walking", false);
        }
    }

    void FixedUpdate()
    {
        if (isChasing && movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
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
