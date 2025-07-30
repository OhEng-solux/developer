using UnityEngine;

public class FlowerBreakTrigger : MonoBehaviour
{
    public GameObject intactFlower;     // 원래 꽃병
    public GameObject brokenFlower;     // 깨진 꽃병
    public KeyCode interactionKey = KeyCode.Z;

    private bool isPlayerNearby = false;
    private bool hasBroken = false;

    void Update()
    {
        if (isPlayerNearby && !hasBroken && Input.GetKeyDown(interactionKey))
        {
            BreakFlower();
        }
    }

    void BreakFlower()
    {
        if (intactFlower != null) intactFlower.SetActive(false);
        if (brokenFlower != null) brokenFlower.SetActive(true);

        hasBroken = true;

        Debug.Log("꽃병 깨짐");
        // 필요하면 추후 추가 이벤트 호출 가능
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}
