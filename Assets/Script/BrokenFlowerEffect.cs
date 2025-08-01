using UnityEngine;
using System.Collections;

public class BrokenFlowerEffect : MonoBehaviour
{
    public GameObject taejuNPC;           // 추격 중인 태주 오브젝트
    public float delayBeforeFade = 0.1f;    // 몇 초 후 사라지게 할지
    public float fadeOutDuration = 1.5f;

    private void OnEnable()
    {
        if (taejuNPC != null && taejuNPC.TryGetComponent(out TaejuChase chase))
        {
            chase.StopChaseAndFreeze(); // 추격 중단 메소드 즉시 호출
        }

        if (taejuNPC != null)
        {
            StartCoroutine(FadeOutTaejuAfterDelay());
        }
    }

    private IEnumerator FadeOutTaejuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        SpriteRenderer sr = taejuNPC.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            float timer = 0f;

            while (timer < fadeOutDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            taejuNPC.SetActive(false);
        }
    }
}