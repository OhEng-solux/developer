using UnityEngine;

public class ItemHintTrigger : MonoBehaviour
{
    public GameObject blueHintText; // 파란색 텍스트만 연결

    void Start()
    {
        blueHintText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            blueHintText.SetActive(true); // 파란 텍스트만 보여줌
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            blueHintText.SetActive(false); // 다시 숨김
        }
    }
}
