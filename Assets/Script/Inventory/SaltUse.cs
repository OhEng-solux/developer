using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ItemUse/Salt")]
public class SaltUse : ItemUseHandler
{
    public override void Use()
    {
        FindFirstObjectByType<SaltUseManager>()?.ActivateEffect();
    }
}