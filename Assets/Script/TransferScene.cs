using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferScene : MonoBehaviour
{
    public string transferMapName;

    private PlayerManager thePlayer;
    private CameraManager theCamera;

    public Transform target;

    public bool flag;
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            thePlayer.currentMapName = transferMapName;
            SceneManager.LoadScene(transferMapName);

        }
    }

}

