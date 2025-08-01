using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{

    // 파괴되지 않게
    static public BGMManager instance;

    public AudioClip[] clips; // 배경 음악들

    private AudioSource source;

    // 반복문 내에서 new가 자주 호출된다면 성능 문제가 생기기 때문에 따로 선언해 주는 게 좋음
    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    private void Awake() // start보다 먼저 실행되는 함수
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void FadeOutMusic() // 음악이 뚝 끊기지 않게
    {
        // in, out이 동시에 발생되지 않도록 
        StopAllCoroutines();
        StartCoroutine(FadeOutMusicCoroutine());
    }

    IEnumerator FadeOutMusicCoroutine()
    {
        for (float i = 1.0f; i >= 0f; i -= 0.01f)
        {
            source.volume = i;
            yield return waitTime;
        }
    }
    public void FadeInMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInMusicCoroutine());
    }

    IEnumerator FadeInMusicCoroutine()
    {
        for (float i = 0f; i <= 1f; i += 0.01f)
        {
            source.volume = i;
            yield return waitTime;
        }
    }

    // Fade Out 후 볼륨이 다시 초기화되게
    public void Play(int _playMusicTrack) // 몇 번 클립 재생할 건지 설정할 파라미터
    {
        source.volume = 1f; // 추가
        source.clip = clips[_playMusicTrack];
        source.Play();
    }

    // 기능 추가
    public void SetVolumn(float _volumn)
    {
        source.volume = _volumn;
    }

    public void Pause() // 일시 정지
    {
        source.Pause();
    }

    public void UnPause() // 일시 정지
    {
        source.UnPause();
    }
}