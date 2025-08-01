using System.Collections;
using UnityEngine;

public class GhostMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveDuration = 3f;
    public float fadeDuration = 2f;

    private bool isMoving = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    public void StartMoving()
    {
        if (!isMoving)
            StartCoroutine(MoveAndFade());
    }

    IEnumerator MoveAndFade()
    {
        isMoving = true;

        float elapsed = 0f;
        Vector2 direction = Vector2.right;


        if (animator != null)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }

        // NPC 
        while (elapsed < moveDuration)
        {
            rb.linearVelocity = direction * moveSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        float fadeElapsed = 0f;
        Color color = sr.color;

        while (fadeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
            sr.color = new Color(color.r, color.g, color.b, alpha);
            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(color.r, color.g, color.b, 0f);

        Destroy(gameObject);
    }
}
