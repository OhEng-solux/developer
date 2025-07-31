using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ItemUse/Key")]
public class KeyUse : ItemUseHandler
{
    public override void Use()
    {
        KeyUseManager keyUseManager = FindFirstObjectByType<KeyUseManager>();
        if (keyUseManager != null)
        {
            keyUseManager.TryUseBasementKey();
        }
        else
        {
            Debug.LogWarning("[KeyUseHandler] KeyUseManager를 찾을 수 없습니다.");
        }
    }
}