using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MovingObject
{
    static public PlayerManager instance; // 정적 변수
    public string currentMapName;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;
    private bool canMove = true;
    public bool notMove = false;

    public bool hasEnteredName = false;

    private float footstepInterval = 0.3f; // 발소리 간격 (초)
    private float lastFootstepTime = 0f;

    private Rigidbody2D rigid;

    void Start()
    {
        queue = new Queue<string>();

        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            theAudio = FindObjectOfType<AudioManager>();
            instance = this;

            boxCollider.offset = new Vector2(0, -0.1f);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator MoveCoroutine()
    {
        while ((Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) && !notMove)
        {
            if (Input.GetKey(KeyCode.Space))
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

            // boxCollider offset 조정 코드 제거됨

            if (currentWalkCount % 3 == 0 && Time.time - lastFootstepTime > footstepInterval)
            {
                int temp = Random.Range(1, 5);
                switch (temp)
                {
                    case 1:
                        theAudio.Play(walkSound_1);
                        break;
                    case 2:
                        theAudio.Play(walkSound_2);
                        break;
                    case 3:
                        theAudio.Play(walkSound_3);
                        break;
                    case 4:
                        theAudio.Play(walkSound_4);
                        break;
                }
                lastFootstepTime = Time.time;
            }

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

                yield return new WaitForSeconds(0.01f);
            }

            currentWalkCount = 0;
        }

        animator.SetBool("Walking", false);
        canMove = true;
    }

    void Update()
    {
        if (canMove && !notMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }
    }
}
