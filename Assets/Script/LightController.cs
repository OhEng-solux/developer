using UnityEngine;

public class LightController : MonoBehaviour
{
    private PlayerManager thePlayer;
    private Vector2 vector;

    void Start()
    {
        gameObject.SetActive(false); // 처음엔 꺼진 상태
    }
    
    void Awake()
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    void Update()
    {
        if (!gameObject.activeSelf || thePlayer == null) return;

        transform.position = thePlayer.transform.position;

        vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));
        // 필요 시 방향에 따른 조명 회전/스프라이트 교체
    }
}