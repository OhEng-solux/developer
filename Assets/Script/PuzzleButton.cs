using UnityEngine;
using UnityEngine.UI;

public class PuzzleButton : MonoBehaviour
{
    public string shape; // ○, △, □, ☆
    public string button_sound; // 인스펙터에서 사운드 name 할당

    private PuzzleManager manager;

    private void Start()
    {
        manager = FindObjectOfType<PuzzleManager>();

        // 버튼 클릭 시 이벤트에 OnButtonClicked 추가
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        // 퍼즐 인풋 전달
        manager.ButtonPressed(shape);

        // 소리 재생: 반드시 클릭에만 실행
        if (!string.IsNullOrEmpty(button_sound) && AudioManager.instance != null)
            AudioManager.instance.Play(button_sound);
    }
}
