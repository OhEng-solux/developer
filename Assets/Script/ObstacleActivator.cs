using UnityEngine;
public class ObstacleActivator : MonoBehaviour
{
    public GameObject target;          
    public int requiredCount = 12;

    void Start()
    {
        target.SetActive(false);       // 벽을 처음엔 비활성화
    }

    void Update()
    {
        if (!target.activeSelf && DialogueProgressManager.instance.dialogueCount >= requiredCount)
        {
            target.SetActive(true);    // 조건 달성 시 벽 등장
        }
    }
}