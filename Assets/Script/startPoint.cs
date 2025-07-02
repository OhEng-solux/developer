using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public string startPoint;//맵 이동 , 플레이어 시작 위치
    private PlayerManager thePlayer;
    private CameraManager theCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        theCamera = FindFirstObjectByType<CameraManager>();
        thePlayer = FindFirstObjectByType<PlayerManager>();
        if (startPoint == thePlayer.currentMapName)//transferMapName이 정보는 다른 스크립트에 존재. 플레이어를 이용한다
        {
            theCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, theCamera.transform.transform.position.z);
            thePlayer.transform.position = this.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
