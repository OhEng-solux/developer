using UnityEngine;

public class AutoSave : MonoBehaviour
{
    public static AutoSave instance;
    private bool flag = true;
    public bool autoSave=false;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (flag == true)
        {
            autoSave = true;
        }
        flag = false;
        //Debug.Log("충돌");
    }
    public bool IsAutoSave()
    {
        return autoSave;
    }

}
