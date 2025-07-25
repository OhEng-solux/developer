using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    public PuzzleManager puzzleManager;

    void Update()
    {
        // Z키 입력 디버깅
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z키 감지: playerInRange = " + playerInRange + ", Active = " + puzzleManager.IsPuzzleActive());
        }

        if (playerInRange && !puzzleManager.IsPuzzleActive() && !puzzleManager.IsPuzzleSolved() && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("퍼즐 시작 조건 충족");
            puzzleManager.StartPuzzle();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
