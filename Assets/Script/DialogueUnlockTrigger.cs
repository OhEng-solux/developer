using UnityEngine;

public class DialogueUnlockTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int requiredCount = 7; // ����: 3�� ��ȭ �� Ʈ���� Ȱ��ȭ
    private BoxCollider2D boxCol;

    void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        boxCol.isTrigger = false; // ���� �� ���α�
    }

    void Update()
    {
        if (!boxCol.isTrigger && DialogueProgressManager.instance.dialogueCount >= requiredCount)
        {
            boxCol.isTrigger = true; // ���� �޼� �� Ʈ���� Ȱ��ȭ
            // (���ϸ� �� ������ 1ȸ�� ����ǵ��� bool �߰�)
        }
    }
}