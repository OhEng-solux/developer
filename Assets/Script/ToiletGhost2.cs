using UnityEngine;
using System.Collections;

public class ToiletGhost2 : MonoBehaviour
{
    public GameObject face;

    private bool isPlayerInside = false;
    private bool isCoroutineRunning = false;
    private bool isFirst = true;

    public float duration = 2f; // 총 지속 시간
    public float startScale = 2f; // 시작 스케일 (2배)
    public float endScale = 1f;   // 끝 스케일 (1배)

    void Start()
    {
        face.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInside && !isCoroutineRunning && Input.GetKeyDown(KeyCode.Z)&& isFirst==true)
        {
            StartCoroutine(ActivateFace());
        }
    }

    private IEnumerator ActivateFace()
    {
        isCoroutineRunning = true;
        face.SetActive(true);
        // 얼굴 2배로 초기화
        face.transform.localScale = Vector3.one * startScale;

        // 스케일 변화 코루틴 시작 (동시에 실행)
        yield return StartCoroutine(ScaleCoroutine());
        yield return new WaitForSeconds(1f);
        // 2초(=duration) 후 얼굴 끄기
        face.SetActive(false);

        // 필요하다면 한번 더 쿨다운을 두려면 (예: yield return new WaitForSeconds(2.5f);) 추가
        yield return new WaitForSeconds(0.5f); // 원하는 시간만큼
        isFirst = false;
        isCoroutineRunning = false;
    }
    private IEnumerator ScaleCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration; // 0~1
            float newScale = Mathf.Lerp(startScale, endScale, t);
            face.transform.localScale = Vector3.one * newScale;
            elapsed += Time.deltaTime;
            yield return null;
        }
        face.transform.localScale = Vector3.one * endScale; // 정확히 1배로 맞춰주기
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            Debug.Log("화장실 온트리거");
            Debug.Log("ToiletGhost2 OnTriggerEnter2D");
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            isPlayerInside = false;
        }
    }
    
}
