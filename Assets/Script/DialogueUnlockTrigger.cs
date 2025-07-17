using UnityEngine;

public class DialogueUnlockTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int requiredCount = 7; // 예시: 3개 대화 후 트리거 활성화
    private BoxCollider2D boxCol;

    void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        boxCol.isTrigger = false; // 시작 땐 꺼두기
    }

    void Update()
    {
        if (!boxCol.isTrigger && DialogueProgressManager.instance.dialogueCount >= requiredCount)
        {
            boxCol.isTrigger = true; // 조건 달성 시 트리거 활성화
            // (원하면 이 구문이 1회만 실행되도록 bool 추가)
        }
    }
}