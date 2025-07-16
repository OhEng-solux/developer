using UnityEngine;

public class DialogueProgressManager : MonoBehaviour
{
    public static DialogueProgressManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public int dialogueCount = 0; // �Ϸ�� ��ȭ ��

    // ��ȭ �Ϸ� �ø��� �� �Լ� ȣ��
    public void AddDialogueCount()
    {
        dialogueCount++;
    }
}