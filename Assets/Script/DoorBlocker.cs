using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorBlocker : MonoBehaviour
{
    public GameObject warningTextObject; // 경고 텍스트 오브젝트
    public float messageDuration = 2f;

    private bool isShowing = false;

    private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"[DoorBlocker] Trigger 감지됨! 충돌한 태그: {other.tag}");

    if (!isShowing && other.CompareTag("Player"))
    {
        isShowing = true;
        warningTextObject.SetActive(true);
        StartCoroutine(HideMessageAfterDelay());
    }
}

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        warningTextObject.SetActive(false);
        isShowing = false;
    }
}
