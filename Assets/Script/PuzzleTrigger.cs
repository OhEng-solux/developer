using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    public PuzzleManager puzzleManager;

    void Update()
    {
        // ZŰ �Է� �����
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("ZŰ ����: playerInRange = " + playerInRange + ", Active = " + puzzleManager.IsPuzzleActive());
        }

        if (playerInRange && !puzzleManager.IsPuzzleActive() && !puzzleManager.IsPuzzleSolved() && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("���� ���� ���� ����");
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
