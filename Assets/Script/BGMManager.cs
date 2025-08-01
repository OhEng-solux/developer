using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    // 싱글톤 구현
    public static BGMManager instance;

    public AudioClip[] clips; // Inspector에서 mp3를 순서대로 할당
    private AudioSource source;
    private int currentBGM = -1;

    // 페이드용 대기시간
    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.loop = true; 
        // 씬 전환 감지 시작
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 실행과 동시에 첫 BGM 적용
        PlayBGMForScene(SceneManager.GetActiveScene().name, instantly: true);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 변경될 때마다 호출됨
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    // ------------ BGM 관리 로직 ------------------

    private int GetBGMIndexForScene(string sceneName)
    {
        if (sceneName == "Start")
            return 0;
        if (sceneName == "Prologue" || sceneName == "Day1" || sceneName == "Day2" || sceneName == "Day3")
            return 1;
        if (sceneName == "Day4" || sceneName == "Day5")
            return 2;
        if (sceneName == "Day6")
            return 3;
        if (sceneName == "Ending_True")
            return 6;
        if (sceneName == "Ending_Hidden")
            return 7;
        if (sceneName == "EndingCredit")
            return 8;
        return -1;
    }

    // 해당 씬 BGM이 다르면 페이드로 교체/없으면 페이드 아웃
    void PlayBGMForScene(string sceneName, bool instantly = false)
    {
        int bgmIdx = GetBGMIndexForScene(sceneName);

        if (bgmIdx == -1)
        {
            StartCoroutine(FadeOutMusicCoroutine());
            currentBGM = -1;
            return;
        }

        if (currentBGM != bgmIdx)
        {
            if (instantly)
            {
                source.volume = 1f;
                Play(bgmIdx);
            }
            else
            {
                StartCoroutine(SwitchBGMWithFade(bgmIdx));
            }
        }
        // 같으면 아무것도 하지 않음 (음악 유지)
    }

    public IEnumerator SwitchBGMWithFade(int nextIdx)
    {
        yield return StartCoroutine(FadeOutMusicCoroutine());
        Play(nextIdx);
        yield return StartCoroutine(FadeInMusicCoroutine());
    }

    public void Play(int _playMusicTrack)
    {
        Debug.Log($"[BGMManager] Play() 호출: {_playMusicTrack} / clips length: {clips.Length}");
        if (_playMusicTrack < 0 || _playMusicTrack >= clips.Length)
        {
            Debug.LogError("[BGMManager] 잘못된 트랙 번호!");
            return;
        }
        if (clips[_playMusicTrack] == null)
        {
            Debug.LogError("[BGMManager] 지정 트랙에 오디오클립이 없음!");
            return;
        }
        currentBGM = _playMusicTrack;
        source.volume = 1f;
        source.clip = clips[_playMusicTrack];
        source.Play();
    }


    public void Stop()
    {
        source.Stop();
        currentBGM = -1;
    }
    public void Pause()
    {
        source.Pause();
    }
    public void UnPause()
    {
        source.UnPause();
    }

    public void SetVolumn(float _volumn)
    {
        source.volume = _volumn;
    }

    public void FadeOutMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutMusicCoroutine());
    }
    public void FadeInMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInMusicCoroutine());
    }

    IEnumerator FadeOutMusicCoroutine()
    {
        float duration = 1.5f; // 총 페이드 시간
        float startVolume = source.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.volume = 0f;
    }


    IEnumerator FadeInMusicCoroutine()
    {
        float duration = 1.5f;
        float targetVolume = 1f;
        source.volume = 0f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }


    // BGMManager.cs

    public IEnumerator FadeOutMusicCoroutinePublic()
    {
        yield return StartCoroutine(FadeOutMusicCoroutine());
    }

    public IEnumerator FadeInMusicCoroutinePublic()
    {
        yield return StartCoroutine(FadeInMusicCoroutine());
    }

}
