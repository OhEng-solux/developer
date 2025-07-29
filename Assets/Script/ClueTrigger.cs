using UnityEngine;

public class ClueTrigger : MonoBehaviour
{
    public int clueIndex; // 0~3
    private static bool[] viewed = new bool[4];
    private static int clueViewedCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !viewed[clueIndex])
        {
            viewed[clueIndex] = true;
            clueViewedCount++;
            Debug.Log($"Clue {clueIndex} viewed. Total: {clueViewedCount}");

            ClueManager.instance?.MarkClueAsRead(clueIndex);
        }
    }
}
