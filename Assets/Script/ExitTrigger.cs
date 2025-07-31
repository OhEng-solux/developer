using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    public enum ExitType { FrontDoor, SideDoor }
    public ExitType exitType;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            TryExit();
        }
    }

    private void TryExit()
    {
        if (ChaseTriggerManager.instance == null)
        {
            Debug.LogWarning("ChaseTriggerManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        if (!ChaseTriggerManager.instance.isChasing) // 추격 시작 여부 확인
        {
            Debug.Log("아직 탈출할 수 없다. 쫓기고 있지 않다.");
            return;
        }
        
        if (ClueManager.instance == null)
        {
            Debug.LogWarning("ClueManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        bool flowerBroken = ClueManager.instance.flowerBroken;

        switch (exitType)
        {
            case ExitType.FrontDoor:
                if (flowerBroken)
                {
                    SceneManager.LoadScene("Ending_True");
                }
                else
                {
                    Debug.Log("꽃병 깨짐 상태가 아님");
                    // 원하면 화면에 안내 문구 출력할 수도 있음
                }
                break;

            case ExitType.SideDoor:
                if (!flowerBroken)
                {
                    SceneManager.LoadScene("Ending_Hidden");
                }
                else
                {
                    Debug.Log("꽃병 깨짐 상태 -> 정문 탈출 유도");
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
