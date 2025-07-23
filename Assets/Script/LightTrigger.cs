using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    public GameObject go;
    private bool flag;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!flag)
        {
            go.SetActive(true);
        }
    }
}
