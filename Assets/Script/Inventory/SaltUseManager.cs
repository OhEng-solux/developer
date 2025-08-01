using UnityEngine;
using System.Collections;

public class SaltUseManager : MonoBehaviour
{
    [SerializeField] private float protectionDuration = 5f;
    [SerializeField] private GameObject shieldEffectObject;

    public void ActivateEffect()
    {
        StartCoroutine(SaltProtectionCoroutine());
    }

    private IEnumerator SaltProtectionCoroutine()
    {
        PlayerManager.instance.isProtectedBySalt = true;
        Debug.Log("[SaltUse] 보호 시작");

        // 이펙트 켜기
        if (shieldEffectObject != null)
            shieldEffectObject.SetActive(true);

        yield return new WaitForSeconds(protectionDuration);

        PlayerManager.instance.isProtectedBySalt = false;
        Debug.Log("[SaltUse] 보호 종료");

        // 이펙트 끄기
        if (shieldEffectObject != null)
            shieldEffectObject.SetActive(false);
    }
}
