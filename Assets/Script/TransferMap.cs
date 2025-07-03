using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string transferMapName;

    private PlayerManager thePlayer;
    private CameraManager theCamera;
    public PolygonCollider2D targetBound;

    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        theCamera = FindFirstObjectByType<CameraManager>();
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            theCamera.SetBound(targetBound);
            thePlayer.currentMapName = transferMapName;
            theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
            thePlayer.transform.position = target.transform.position;
        }
    }
}
