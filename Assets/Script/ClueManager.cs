using UnityEngine;
using System.Collections; 

public class ClueManager : MonoBehaviour
{
    public static ClueManager instance;

    private bool[] clueRead = new bool[4];
    private int clueCount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void MarkClueAsRead(int index)
    {
        if (index < 0 || index >= clueRead.Length) return;

        if (!clueRead[index])
        {
            clueRead[index] = true;
            clueCount++;

            Debug.Log($"Clue {index} 확인됨. 총 단서 수: {clueCount}");

            if (clueCount >= clueRead.Length)
            {
                Debug.Log("모든 단서 확인 완료 → 2초 후 추격 대사 시작");
                StartCoroutine(DelayedStartChaseDialogue());
            }
        }
    }

    private IEnumerator DelayedStartChaseDialogue()
    {
        yield return new WaitForSeconds(1.5f); // ⏱️ 2초 대기

        var chaseTrigger = FindFirstObjectByType<ChaseTriggerManager>();
        if (chaseTrigger != null)
        {
            chaseTrigger.StartChaseDialogue();
        }
        else
        {
            Debug.LogError("ChaseTriggerManager를 찾을 수 없습니다.");
        }
    }
}
