using UnityEngine;

public class ToyPuzzleTrigger : MonoBehaviour
{
    public GameObject puzzlePanel; // 퍼즐 UI 패널
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z키 눌림 - 퍼즐 시작");
            puzzlePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
