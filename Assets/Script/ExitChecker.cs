using System.Collections;
using UnityEngine;

public class ExitChecker : MonoBehaviour
{
    public Dialogue warningDialogue;        // "���� ��ȭ�� �� ������ ���� �� ����." ���
    public Transform targetPosition;        // �̵���ų ��ġ
    public string targetMapName;            // �̵��� �� �̸�
    public PolygonCollider2D targetBound;   // �̵��� ���� Bound ����

    private CameraManager theCamera;
    private FadeManager theFade;
    private OrderManager theOrder;
    private PlayerManager thePlayer;

    void Start()
    {
        theCamera = FindFirstObjectByType<CameraManager>();
        theFade = FindFirstObjectByType<FadeManager>();
        theOrder = FindFirstObjectByType<OrderManager>();
        thePlayer = FindFirstObjectByType<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (DialogueProgressManager.instance.dialogueCount < 9)
            {
                if (!DialogueManager.instance.talking)
                {
                    DialogueManager.instance.ShowDialogue(warningDialogue, false); // ��ȭ�� ���
                }
            }
            else
            {
                StartCoroutine(TransferCoroutine());
            }
        }
    }

    private IEnumerator TransferCoroutine()
    {
        theOrder.NotMove();
        theFade.FadeOut();
        yield return new WaitForSeconds(1f);

        thePlayer.currentMapName = targetMapName;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(
            targetPosition.position.x,
            targetPosition.position.y,
            theCamera.transform.position.z
        );
        thePlayer.transform.position = targetPosition.position;

        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }
}
