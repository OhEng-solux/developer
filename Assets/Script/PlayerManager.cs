using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MovingObject
{
    static public PlayerManager instance; //정적변수
    public string currentMapName;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;
    private bool canMove = true;

    private Rigidbody2D rigid;

    void Start()
    {
        queue = new Queue<string>();

        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed;
                applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                applyRunFlag = false;
            }

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

            if (vector.x != 0) vector.y = 0;

            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            bool checkCollisionFlag = base.CheckCollision();
            if (checkCollisionFlag) break;

            animator.SetBool("Walking", true);

            // boxCollider 위치 조정
            boxCollider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * (speed + applyRunSpeed), 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * (speed + applyRunSpeed), 0);
                }

                if (applyRunFlag) currentWalkCount++;
                currentWalkCount++;

                if (currentWalkCount == 12)
                    boxCollider.offset = Vector2.zero;

                yield return new WaitForSeconds(0.01f);
            }

            currentWalkCount = 0;
        }

        animator.SetBool("Walking", false);
        canMove = true;
    }

    void Update()
    {
        if (canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }
    }
}