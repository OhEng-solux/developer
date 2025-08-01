using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferScene : MonoBehaviour
{
    public string transferSceneName;

    private CameraManager theCamera;
    [SerializeField] private PlayerManager thePlayer;
    private FadeManager theFade;
    private OrderManager theOrder;

    public GameObject blockerObject; // 못 지나가게 막기 위함

    // Start is called before the first frame update
    void Start()
    {
        theFade = FindFirstObjectByType<FadeManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
        theCamera = FindFirstObjectByType<CameraManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(TransferCoroutine());
        }
    }

    IEnumerator TransferCoroutine()
    {
        theOrder.NotMove();
        theFade.FadeOut();
        yield return new WaitForSeconds(1f);
        thePlayer.currentSceneName = transferSceneName;
        AsyncOperation ao = SceneManager.LoadSceneAsync(transferSceneName);
        while (!ao.isDone)
            yield return null;
    }

    void SomeConditionMet()
    {
        Debug.Log("조건 실행! blocker 활성 전: " + blockerObject.activeSelf);
        blockerObject.SetActive(false);
        Debug.Log("blocker 비활성 후: " + blockerObject.activeSelf);
    }

}

