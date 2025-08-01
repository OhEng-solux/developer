using UnityEngine;

public class BGMTest : MonoBehaviour
{
    BGMManager BGM;

    public int playMusicTrack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BGM = FindObjectOfType<BGMManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BGM.Play(playMusicTrack);
        this.gameObject.SetActive(false); // 컬라이더가 한 번 작동되면 꺼짐 (한 번 재생되면 쭉!)
    }
}