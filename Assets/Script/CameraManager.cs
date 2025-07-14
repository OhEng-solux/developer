using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager: MonoBehaviour
{
    static public CameraManager instance;
    public GameObject target; //카메라가 따라갈 대상
    public float moveSpeed; //카메라가 대상을 쫓을 속도
    private Vector3 targetPosition; //대상의 현재 위치값

    public PolygonCollider2D bound;
    private Vector3 minBound;
    private Vector3 maxBound;
    //박스 콜라이더 영역의 최소 최대 xyz값을 지님

    private float halfWidth;
    private float halfHeight;
    //카메라의 반너비, 반높이 값을 지닐 변수
    
    Vector3 velocity = Vector3.zero;  // SmoothDamp 속도 캐시용
    public float moveSmoothTime = 0.15f;   // SmoothDamp용

    private Camera theCamera; //카메라의 반높이값을 구할 속성을 이용하기 위한 변수

    private void Awake()
    {
        if(instance!=null){
            Destroy(this.gameObject);
        }
        else{
            DontDestroyOnLoad(this.gameObject);
            instance=this;
        }
    }
    void Start()
    {
        theCamera = Camera.main;

        if (bound == null) // bound 체크
        {
            return;
        }

        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;

        halfHeight = theCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;
    }

    void LateUpdate()
    {
        if (target == null || bound == null) return;

        Vector3 desired = target.transform.position;
        desired.z = transform.position.z;     // z 고정

        // 경계 값(카메라 크기를 고려해 한 번만 계산)
        float minX = minBound.x + halfWidth;
        float maxX = maxBound.x - halfWidth;
        float minY = minBound.y + halfHeight;
        float maxY = maxBound.y - halfHeight;

        // “진짜 클램프를 해야 하나?” 판단 ---------------------------------
        bool roomLeft = desired.x - minBound.x >= halfWidth;
        bool roomRight = maxBound.x - desired.x >= halfWidth;
        bool roomDown = desired.y - minBound.y >= halfHeight;
        bool roomUp = maxBound.y - desired.y >= halfHeight;

        bool clampX = roomLeft && roomRight;   // → 두 쪽 모두 여유가 있어야 클램프
        bool clampY = roomDown && roomUp;      // → 위·아래도 마찬가지
                                               // --------------------------------------------------------------

        // 좌표 계산
        float nextX = clampX ? Mathf.Clamp(desired.x, minX, maxX) : desired.x;
        float nextY = clampY ? Mathf.Clamp(desired.y, minY, maxY) : desired.y;
        Vector3 targetPos = new Vector3(nextX, nextY, desired.z);

        // 부드럽게 or 즉시
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            moveSmoothTime
        );
    }



    public void SetBound(PolygonCollider2D newBound){
        bound=newBound;
        minBound=bound.bounds.min;
        maxBound=bound.bounds.max;

        float width = maxBound.x - minBound.x;
        float height = maxBound.y - minBound.y;

        // 최소 크기보다 작으면 오류 방지
        if (width < halfWidth * 2)
        {
            float centerX = (minBound.x + maxBound.x) / 2f;
            minBound.x = centerX - halfWidth;
            maxBound.x = centerX + halfWidth;
        }

        if (height < halfHeight * 2)
        {
            float centerY = (minBound.y + maxBound.y) / 2f;
            minBound.y = centerY - halfHeight;
            maxBound.y = centerY + halfHeight;
        }
    }
}
