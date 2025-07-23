using UnityEngine;

public class ToyPuzzleTrigger : MonoBehaviour
{
    public GameObject puzzlePanel; // ���� UI �г�
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("ZŰ ���� - ���� ����");
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
