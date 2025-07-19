using System.Collections;
using UnityEngine;

public class ExitChecker : MonoBehaviour
{
    public Dialogue warningDialogue;        // "���� ��ȭ�� �� ������ ���� �� ����." ���
    public Transform targetPosition;        // �̵���ų ��ġ
    public string targetMapName;            // �̵��� �� �̸�
    public PolygonCollider2D targetBound;   // �̵��� ���� Bound ����

    [Tooltip("�ʿ��� �ּ� ��ȭ ���� �� (�⺻ 9)")]
    public int requiredDialogueCount = 9;   // �⺻�� 9

    private CameraManager theCamera;
    private FadeManager theFade;
    private OrderManager theOrder;
    private PlayerManager thePlayer;

    private float talkCooldown = 1f; // ���ȭ ���� �ð� ���� (��)
    private float lastTalkTime = -10f; // ������ ��ȭ �ð� �ʱ�ȭ

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
            float currentTime = Time.time;

            if (DialogueProgressManager.instance.dialogueCount < requiredDialogueCount)
            {
                if (!DialogueManager.instance.talking && currentTime - lastTalkTime > talkCooldown)
                {
                    lastTalkTime = currentTime;
                    DialogueManager.instance.ShowDialogue(warningDialogue, false); // ��ȭ�� ���
                }
            }
            else
            {
                // ��Ÿ�� ���� �ٷ� �̵� ó��
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
