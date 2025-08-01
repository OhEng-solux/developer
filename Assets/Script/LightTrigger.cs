using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    public GameObject go; // 라이트 오브젝트
    private bool flag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag && collision.CompareTag("Player")) // 플레이어일 경우만
        {
            go.SetActive(true); // 라이트 켜기
            flag = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            flag = false; // 플레이어가 나가면 flag 초기화
        }
    }
}