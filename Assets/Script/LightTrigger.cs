using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    public GameObject go; // ï¿½ï¿½ï¿½ï¿½Æ® ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Æ®
    private bool flag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag && collision.CompareTag("Player")) // ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ï¿½ï¿½ ï¿½ï¿½ì¸¸
        {
            go.SetActive(true); // ï¿½ï¿½ï¿½ï¿½Æ® ï¿½Ñ±ï¿½
            flag = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            flag = false; // ÇÃ·¹ÀÌ¾î°¡ ³ª°¡¸é flag ÃÊ±âÈ­
        }
    }
}