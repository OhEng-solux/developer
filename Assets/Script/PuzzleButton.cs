using UnityEngine;
using UnityEngine.UI;

public class PuzzleButton : MonoBehaviour
{
    public string shape; // ��, ��, ��, ��
    public string button_sound; // �ν����Ϳ��� ���� name �Ҵ�

    private PuzzleManager manager;

    private void Start()
    {
        manager = FindObjectOfType<PuzzleManager>();

        // ��ư Ŭ�� �� �̺�Ʈ�� OnButtonClicked �߰�
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        // ���� ��ǲ ����
        manager.ButtonPressed(shape);

        // �Ҹ� ���: �ݵ�� Ŭ������ ����
        if (!string.IsNullOrEmpty(button_sound) && AudioManager.instance != null)
            AudioManager.instance.Play(button_sound);
    }
}
