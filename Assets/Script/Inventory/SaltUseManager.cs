using UnityEngine;
using System.Collections;

public class SaltUseManager : MonoBehaviour
{
    [SerializeField] private float protectionDuration = 5f;

    public void ActivateEffect()
    {
        StartCoroutine(SaltProtectionCoroutine());
    }

    private IEnumerator SaltProtectionCoroutine()
    {
        PlayerManager.instance.isProtectedBySalt = true;
        Debug.Log("[SaltUse] 보호 시작");

        yield return new WaitForSeconds(protectionDuration);

        PlayerManager.instance.isProtectedBySalt = false;
        Debug.Log("[SaltUse] 보호 종료");
    }
}
