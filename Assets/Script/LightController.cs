using UnityEngine;

public class LightController : MonoBehaviour
{
    private PlayerManager thePlayer;
    private Vector2 vector;
    private Quaternion rotation;

    void Start()
    {
        gameObject.SetActive(false);
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    void Update()
    {
        // ������Ʈ�� ��Ȱ��ȭ�Ǿ��� ��� ���� X
        if (!gameObject.activeInHierarchy || thePlayer == null) return;

        transform.position = thePlayer.transform.position;

        vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));

        if (vector.x == 1f)
            rotation = Quaternion.Euler(0, 0, 90);
        else if (vector.x == -1f)
            rotation = Quaternion.Euler(0, 0, -90);
        else if (vector.y == 1f)
            rotation = Quaternion.Euler(0, 0, 180);
        else if (vector.y == -1f)
            rotation = Quaternion.Euler(0, 0, 0);

        transform.rotation = rotation;
    }
}