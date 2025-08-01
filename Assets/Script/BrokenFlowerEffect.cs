using UnityEngine;
using System.Collections;

public class BrokenFlowerEffect : MonoBehaviour
{
    public GameObject taejuNPC;           // 추격 중인 태주 오브젝트
    public Transform flowerPosition;   // 꽃병 위치
    public float offsetDistance = 1.5f; // 꽃병에서 떨어질 거리
    public float fadeOutDuration = 1.5f;
    public Sprite taejuFacingFrontSprite;

    public Dialogue[] endingDialogues;    // 꽃병 깨진 후 대사들

    private void OnEnable()
    {
        AudioManager.instance.Play("vase_break");
        if (taejuNPC != null && taejuNPC.TryGetComponent(out TaejuChase chase))
        {
            chase.StopChaseAndFreeze(); // 추격 중단

            if (flowerPosition != null)
            {
                Vector3 offset = new Vector3(offsetDistance, 0f, 0f); // 오른쪽으로
                // taejuNPC.transform.position = flowerPosition.position + offset;

                SpriteRenderer sr = taejuNPC.GetComponent<SpriteRenderer>();
                if (sr != null && taejuFacingFrontSprite != null)
                {
                    sr.sprite = taejuFacingFrontSprite;
                }

                Animator animator = taejuNPC.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = false; // Animator 비활성화
                }
            }
        }

        if (endingDialogues != null && endingDialogues.Length > 0)
        {
            StartCoroutine(PlayDialogueAndFadeOut());
        }
        else
        {
            StartCoroutine(FadeOutTaeju());
        }
    }

    private IEnumerator PlayDialogueAndFadeOut()
    {
        for (int i = 0; i < endingDialogues.Length; i++)
        {
            DialogueManager.instance.ShowDialogue(endingDialogues[i], shouldCount: false);
            yield return new WaitUntil(() => !DialogueManager.instance.talking);
        }
        ClueManager.instance.endingReady = true; // 진엔딩 준비 완료 상태로 설정

        yield return FadeOutTaeju();
    }

    private IEnumerator FadeOutTaeju()
    {
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