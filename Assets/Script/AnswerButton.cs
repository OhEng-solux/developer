using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    [Tooltip("○, △, □, ☆")]
    public string shapeValue;
    public string button_sound;

    [Header("버튼 개별 이미지")]
    public Sprite normalSprite;   // 기본 상태 이미지
    public Sprite pressedSprite;  // 눌린 상태 이미지
}
