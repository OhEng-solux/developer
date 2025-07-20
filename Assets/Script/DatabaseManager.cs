using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //
    static public DatabaseManager instance; // �̱��� ������ ���� ���� // Scene �̵� ������ AudioManager�� �ı����� �ʵ��� �ϱ� ���� static���� ����

    void Awake()
    {
        if (instance != null) // instance�� null�̸� // CameraManager.cs�� ó�� ����� ���� null��
        {
            Destroy(this.gameObject); // �̹� �����ϴ� ��� �ı�
        }
        else
        {
            DontDestroyOnLoad(this.gameObject); // ���� �ٲ� �ı����� ����
            instance = this; // instance�� �ڱ� �ڽ��� ����
        }
    }

    public string[] var_name;
    public string[] var;
    public string[] vswitch_name;
    public bool switches;


    void Start()
    {
        
    }


}
