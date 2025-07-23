using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightController : MonoBehaviour
{
    private PlayerManager thePlayer; // 플레이어가 바라보고 있는 방향
    private Vector2 vector;

    private Quaternion rotation; // 회전(각도)을 담당하는 Vector4 x y z w

    void Start()
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = thePlayer.transform.position; // 플레이어의 위치에 따라 라이트 위치 변경
        vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));

        if(vector.x == 1f)
        {
            rotation = Quaternion.Euler(0, 0, 90); // 오른쪽
            this.transform.rotation = rotation;
        }
        else if(vector.x == -1f)
        {
            rotation = Quaternion.Euler(0, 0, -90); // 왼쪽
            this.transform.rotation = rotation;
        }
        else if(vector.y == 1f)
        {
            rotation = Quaternion.Euler(0, 0, 180); // 위쪽
            this.transform.rotation = rotation;
        }
        else if(vector.y == -1f)
        {
            rotation = Quaternion.Euler(0, 0, 0); // 아래쪽
            this.transform.rotation = rotation;
        }
    }
}
