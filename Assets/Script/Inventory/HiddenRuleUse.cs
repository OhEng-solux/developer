using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ItemUse/HiddenRule")]
public class HiddenRuleUse : ItemUseHandler
{
    public override void Use()
    {
        // UI를 담당하는 MonoBehaviour에 명령
        HiddenRuleViewer.Instance.OpenViewer();
    }
}