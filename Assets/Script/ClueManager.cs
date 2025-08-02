using UnityEngine;
using System.Collections; 

public class ClueManager : MonoBehaviour
{
    public static ClueManager instance;

    private bool[] clueRead = new bool[4];
    public int clueCount = 0;

    public bool chaseStarted = false; // 추격 시작 여부

    public bool flowerBroken = false; // 꽃병 깨짐 상태
    public bool endingReady = false; // -> 추격 대사 시작 후 진엔딩 준비 완료 여부
    private bool flag = true;
    void Update()
    {
        if (clueCount >= 4&&flag==true)
        {
            StartCoroutine(DelayedStartChaseDialogue());
            flag = false;
        }
    }
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
                SaveManager.instance.StartCoroutine(SaveManager.instance.OpenSave());
                PlayerManager.instance.canMove = false;
                Debug.Log("플레이어 멈춤");
                StartCoroutine(DelayedStartChaseDialogue());
                Debug.Log("모든 단서 확인 완료 → 2초 후 추격 대사 시작");
                
            }
            
        }
    }

    private IEnumerator DelayedStartChaseDialogue()
    {
        yield return new WaitWhile(() => ((SaveManager.instance != null && SaveManager.instance.IsSaveActive())|| (ImagePopupManager.instance != null && ImagePopupManager.instance.IsImageActive())));
        yield return new WaitForSeconds(2f); // ⏱️ 2초 대기
        var chaseTrigger = FindFirstObjectByType<ChaseTriggerManager>();
        if (chaseTrigger != null)
        {
            chaseTrigger.StartChaseDialogue();
        }
        else
        {
            Debug.LogError("ChaseTriggerManager를 찾을 수 없습니다.");
        }
        Debug.Log("플레이어 움직임");
        if (PlayerManager.instance != null) 
           PlayerManager.instance.canMove = true;
    }

}
