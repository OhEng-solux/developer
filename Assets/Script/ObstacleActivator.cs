using UnityEngine;
public class ObstacleActivator : MonoBehaviour
{
    public GameObject target;          
    public int requiredCount = 12;

    void Start()
    {
        target.SetActive(false);       // ���� ó���� ��Ȱ��ȭ
    }

    void Update()
    {
        if (!target.activeSelf && DialogueProgressManager.instance.dialogueCount >= requiredCount)
        {
            target.SetActive(true);    // ���� �޼� �� �� ����
        }
    }
}