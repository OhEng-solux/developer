using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public string startPoint;//�� �̵� , �÷��̾� ���� ��ġ
    private PlayerManager thePlayer;
    private CameraManager theCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        theCamera = FindFirstObjectByType<CameraManager>();
        thePlayer = FindFirstObjectByType<PlayerManager>();
        if (startPoint == thePlayer.currentMapName)//transferMapName�� ������ �ٸ� ��ũ��Ʈ�� ����. �÷��̾ �̿��Ѵ�
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
