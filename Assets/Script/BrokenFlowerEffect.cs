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
        if (taejuNPC != null && taejuNPC.TryGetComponent(out TaejuChase chase))
        {
            chase.StopChaseAndFreeze();  

            chase.recordedPositions.Clear(); // 위치 기록 철저히 비우기
            chase.PauseChase(true);          // pauseChase flag로 완전 멈춤
            chase.DisableColliderTemporarily(999f); // 콜라이더도 비활성

            // ------------ 여기서만 BGM 코루틴 호출 ----------
            if (BGMManager.instance != null)
            {
                StartCoroutine(chase.BackToDay6BGM());
            }
            //--------------------------------------------------

            if (flowerPosition != null)
            {
                // 위치 이동/외형 처리
                Vector3 offset = new Vector3(offsetDistance, 0f, 0f);
                // 필요한 경우만 위치 보정
                // taejuNPC.transform.position = flowerPosition.position + offset;

                SpriteRenderer sr = taejuNPC.GetComponent<SpriteRenderer>();
                if (sr != null && taejuFacingFrontSprite != null)
                    sr.sprite = taejuFacingFrontSprite;

                Animator animator = taejuNPC.GetComponent<Animator>();
                if (animator != null)
                    animator.enabled = false;
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