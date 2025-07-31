using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    public GameObject go; // ����Ʈ ������Ʈ
    private bool flag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag && collision.CompareTag("Player")) // �÷��̾��� ��츸
        {
            go.SetActive(true); // ����Ʈ �ѱ�
            flag = true;
        }
    }
}