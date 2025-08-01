using UnityEngine;
using System.Collections;

public class CandyUseManager : MonoBehaviour
{
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private GameObject freezeEffectObject;

    public void ActivateEffect()
    {
        StartCoroutine(StunChaserCoroutine());
    }

    private IEnumerator StunChaserCoroutine()
    {
        TaejuChase chaser = FindFirstObjectByType<TaejuChase>();
        if (chaser != null && chaser.IsChasing())
        {
            chaser.StopChase();
            Debug.Log("[CandyUse] 추격자 정지 시작");
            AudioManager.instance.Play("candy_use");

            // 이펙트 켜기
            if (freezeEffectObject != null)
                freezeEffectObject.SetActive(true);

            yield return new WaitForSeconds(stunDuration);

            chaser.StartChase();
            chaser.PauseChase(false); // Candy로 멈춘 후, 일시정지 상태는 강제로 풀어줌
            Debug.Log("[CandyUse] 추격자 정지 해제");

            // 이펙트 끄기
            if (freezeEffectObject != null)
                freezeEffectObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[CandyUse] 추격자가 없거나 추격 중이 아님");
        }
    }
}
