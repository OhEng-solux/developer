using UnityEngine;

public class LightController : MonoBehaviour
{
    private PlayerManager thePlayer;
    private Vector2 vector;

    void Start()
    {
        gameObject.SetActive(false);
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    void Update()
    {
        // 오브젝트가 비활성화되었을 경우 실행 X
        if (!gameObject.activeInHierarchy || thePlayer == null) return;

        transform.position = thePlayer.transform.position;

        // 플레이어 방향 값 가져오기 (필요 시 다른 처리에 사용)
        vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));

        // 이미지 변경 방식이라면 방향에 따른 Sprite 교체 처리 등을 여기에 작성
    }
}
