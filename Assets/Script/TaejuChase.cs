using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public Queue<Vector3> recordedPositions = new Queue<Vector3>();
    private float timer;
    private bool isChasing = false;
    private bool pauseChase = false;

    private Collider2D chaseCollider;
    public bool isFading = false;  // 페이드 진행중 체크용 플래그

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        chaseCollider = GetComponent<Collider2D>();

        if (player == null)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found) player = found.transform;
        }

        // 회전 금지
        rb.freezeRotation = true;

        // 태주-플레이어 충돌 무시
        /*
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("character"),
            LayerMask.NameToLayer("character"),
            true
        );
        */
    }

    void Update()
    {
        if (!isChasing || pauseChase || player == null) return;

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
        if (!isChasing || pauseChase || recordedPositions.Count == 0) return;

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
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        animator.SetBool("Walking", true);
        StartCoroutine(ChangeToChaseBGM());
    }

    private IEnumerator ChangeToChaseBGM()
    {
        if (BGMManager.instance != null)
        {
            yield return BGMManager.instance.StartCoroutine(BGMManager.instance.FadeOutMusicCoroutinePublic());
            BGMManager.instance.Play(5); // chase.mp3 인덱스
            yield return BGMManager.instance.StartCoroutine(BGMManager.instance.FadeInMusicCoroutinePublic());
        }
    }


    public void StopChase()
    {
        isChasing = false;
        animator.SetBool("Walking", false);
        // StartCoroutine(BackToDay6BGM());
    }

    public void PauseChase(bool isPaused)
    {
        pauseChase = isPaused;

        if (isPaused)
            animator.SetBool("Walking", false);
    }

    public bool IsChasing()
    {
        return isChasing;
    }

    public void SpawnAtWithDelay(Vector3 position, float delay)
    {
        StartCoroutine(SpawnAfterDelay(position, delay));
    }
    private IEnumerator SpawnAfterDelay(Vector3 position, float delay)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        isFading = true; // 이 동안 잡기 방지

        transform.position = position; // ← 반드시 이 줄이 delay 전에 있나 확인!

        yield return new WaitForSeconds(delay);

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isFading = false;
        StartChase();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerManager.instance != null && PlayerManager.instance.isInvincible)
            return;

        if (collision.CompareTag("Player"))
        {
            if (isFading) return;

            if (PlayerManager.instance != null && PlayerManager.instance.isProtectedBySalt)
            {
                Debug.Log("[SaltUse] 보호 상태 - 배드엔딩 무시됨");
                return;
            }
            // **여기서 바로 씬 이동하지 말고 페이드 연출 시작**
            StartCoroutine(BadEndingTransition());
        }
    }

    private IEnumerator BadEndingTransition()
    {
        // 1. BGM 페이드 아웃
        if (BGMManager.instance != null)
        {
            yield return BGMManager.instance.StartCoroutine(BGMManager.instance.FadeOutMusicCoroutinePublic());
        }
        // 2. 베드엔딩 씬 이동
        yield return new WaitForSeconds(0.2f); // 연출상 잠깐 멈추고 싶을 때
        SceneManager.LoadSceneAsync("Ending_Bad");
    }

    public void StopChaseAndFreeze()
    {
        StopChase();
        pauseChase = true;             // 이동 차단 플래그 활성화
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic; // 물리 계산 중단
        Debug.Log("태주 멈춤");
    }

    public void DisableColliderTemporarily(float seconds)
    {
        if (chaseCollider != null)
        {
            chaseCollider.enabled = false;
            StartCoroutine(ReenableColliderAfterDelay(seconds));
        }
    }

    private IEnumerator ReenableColliderAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (chaseCollider != null)
            chaseCollider.enabled = true;
    }


    public IEnumerator BackToDay6BGM()
    {
        Debug.Log("BackToDay6BGM 호출, BGM 2번 트라이");
        if (BGMManager.instance != null)
        {
            yield return BGMManager.instance.StartCoroutine(BGMManager.instance.FadeOutMusicCoroutinePublic());
            BGMManager.instance.Play(2);
            yield return BGMManager.instance.StartCoroutine(BGMManager.instance.FadeInMusicCoroutinePublic());
        }
    }

}