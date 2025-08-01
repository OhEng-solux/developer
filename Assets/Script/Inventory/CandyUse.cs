using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ItemUse/Candy")]
public class CandyUse : ItemUseHandler
{
    public override void Use()
    {
        FindFirstObjectByType<CandyUseManager>()?.ActivateEffect();
    }
}