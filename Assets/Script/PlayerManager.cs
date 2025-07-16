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
    public bool notMove = false;

    private float footstepInterval = 0.3f; // 발소리 간격 (초)
    private float lastFootstepTime = 0f;

    public bool hasEnteredName = false;

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
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0 && !notMove)
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

            // boxCollider 위치 조정
            boxCollider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

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
        if (canMove && !notMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                // "Horizontal" 우 방향키가 눌리면 1 리턴, 좌 방향키가 눌리면 -1 리턴
                // "Vertical"인 경우, 상은 1 리턴, 하는 -1 리턴
                canMove = false;
                StartCoroutine(MoveCoroutine()); // 방향키를 누르는 순간 동시에 여러 개 실행됨
            }
        }
    }
}