using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //
    static public DatabaseManager instance; // 싱글톤 패턴을 위한 변수 // Scene 이동 간에도 AudioManager가 파괴되지 않도록 하기 위해 static으로 설정

    void Awake()
    {
        if (instance != null) // instance가 null이면 // CameraManager.cs가 처음 실행될 때만 null임
        {
            Destroy(this.gameObject); // 이미 존재하는 경우 파괴
        }
        else
        {
            DontDestroyOnLoad(this.gameObject); // 씬이 바뀌어도 파괴되지 않음
            instance = this; // instance에 자기 자신을 대입
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
