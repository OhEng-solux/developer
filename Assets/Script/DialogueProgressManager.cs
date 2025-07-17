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

    public int dialogueCount = 0; // 완료된 대화 수

    // 대화 완료 시마다 이 함수 호출
    public void AddDialogueCount()
    {
        dialogueCount++;
    }
}