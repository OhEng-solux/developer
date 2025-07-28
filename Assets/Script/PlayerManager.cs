using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MovingObject
{
    static public PlayerManager instance; // 정적 변수

    public string currentMapName;
    public string currentSceneName;
    public GameObject dayStartImage;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;

    public bool canMove = true;
    public bool notMove = false;

    private FadeManager theFade;
    public bool hasEnteredName = false;

    private float footstepInterval = 0.3f; // 발소리 간격 (초)
    private float lastFootstepTime = 0f;

    private TestDialogue[] allDialogues;
    private Rigidbody2D rigid;

    // **추가: queue, currentWalkCount 필드 선언**
    private Queue<string> queue;
    private int currentWalkCount = 0;

    IEnumerator Start()
    {
        if (gameObject.scene.name == "Start" || gameObject.scene.name == "Prologue")
        {
            Debug.Log("시작화면");
            yield break;
        }

        TestDialogue[] allDialogues = FindObjectsOfType<TestDialogue>(true);
        foreach (var dialogue in allDialogues)
        {
            dialogue.gameObject.SetActive(false); // 각 오브젝트 비활성화
        }

        dayStartImage.SetActive(true); // day N 켜기
        Time.timeScale = 0f; // 게임 일시정지

        queue = new Queue<string>(); // 필드 변수 초기화

        yield return new WaitForSecondsRealtime(2.5f); // 2.5초 대기
        dayStartImage.SetActive(false); // day N 끄기

        Time.timeScale = 1f; // 게임 재개

        theFade = FindFirstObjectByType<FadeManager>();
        Debug.Log("fadein");
        theFade.FadeIn();
        yield return new WaitForSecondsRealtime(1f);

        Debug.Log("대화 가능");
        foreach (var dialogue in allDialogues)
        {
            dialogue.gameObject.SetActive(true); // 각 오브젝트 활성화
        }

        if (instance == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            theAudio = FindFirstObjectByType<AudioManager>();
            rigid = GetComponent<Rigidbody2D>();
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
        currentWalkCount = 0; // 이동 카운트 초기화

        while ((Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) && !notMove)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
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
                Vector2 newPosition = rigid.position;

                if (vector.x != 0)
                {
                    newPosition += new Vector2(vector.x * (speed + applyRunSpeed), 0);
                }
                else if (vector.y != 0)
                {
                    newPosition += new Vector2(0, vector.y * (speed + applyRunSpeed));
                }

                rigid.MovePosition(newPosition);

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
        // UI 팝업, 세이브, 인벤토리 등 활성화 상태 일 때 입력 완전히 차단
        if ((PopupManager.instance != null && PopupManager.instance.IsPopupActive())
            || (SaveManager.instance != null && SaveManager.instance.IsSaveActive())
            || (InventoryManager.instance != null && InventoryManager.instance.isOpen))
        {
            return;
        }

        if (!canMove) return;

        if (gameObject.scene.name == "Start" || gameObject.scene.name == "Prologue")
        {
            Debug.Log("시작화면");
            return;
        }

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
