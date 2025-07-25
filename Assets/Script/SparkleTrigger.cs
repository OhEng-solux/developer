using UnityEngine;

public class SparkleTrigger : MonoBehaviour
{
    public Sprite popupImage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ImagePopupManager.instance.ShowPopup(popupImage);
            Destroy(this); // �ݺ� ������, ���ϸ� ����
        }
    }
}