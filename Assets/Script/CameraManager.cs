using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager: MonoBehaviour
{
    static public CameraManager instance;
    public GameObject target; //카메라가 따라갈 대상
    public float moveSpeed; //카메라가 대상을 쫓을 속도
    private Vector3 targetPosition; //대상의 현재 위치값

    public BoxCollider2D bound;
    private Vector3 minBound;
    private Vector3 maxBound;
    //박스 콜라이더 영역의 최소 최대 xyz값을 지님

    private float halfWidth;
    private float halfHeight;
    //카메라의 반너비, 반높이 값을 지닐 변수

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
    void Start(){
        theCamera=GetComponent<Camera>();
        //맵 이동 구현 시 활성화
        //minBound=bound.bounds.min;
        //maxBound=bound.bounds.max;
        halfHeight =theCamera.orthographicSize;
        halfWidth=halfHeight * Screen.width / Screen.height;
    }
    void Update(){
        if (target.gameObject != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);
            //this는 이 script가 적용될 객체, 지금은 카메라, 생략 가능
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            //1초에 movespeed만큼 이동

            //맵 이동 구현 시 활성화
            //float clampedX=Mathf.Clamp(this.transform.position.x, minBound.x+halfWidth, maxBound.x-halfWidth);
            //float clampedY=Mathf.Clamp(this.transform.position.y, minBound.y+halfHeight, maxBound.y-halfHeight);

            //this.transform.position=new Vector3(clampedX, clampedY, this.transform.position.z);
        }
    }

    public void SetBound(BoxCollider2D newBound){
        bound=newBound;
        minBound=bound.bounds.min;
        maxBound=bound.bounds.max;
    }
}
