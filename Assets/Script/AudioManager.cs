using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] // custom class 를 인스펙터에서 편집할 수 있도록 설정

public class Sound
{
    public string name; // 사운드의 이름
    public AudioClip clip; // 사운드의 파일
    public AudioSource source; // 사운드 플레이어
    public float Volume; // 사운드의 볼륨
    public bool loop; // 사운드의 반복 여부

    public void SetSource(AudioSource _source)
    {
        source = _source; // 사운드 플레이어를 설정
        source.clip = clip; // 사운드 파일을 설정
        source.volume = Volume; // 사운드 볼륨을 설정
        source.loop = loop; // 사운드 반복 여부를 설정
    }

    public void SetVolume()
    {
        source.volume = Volume; // 사운드 볼륨을 설정
    }
    public void Play()
    {
        source.Play(); // 사운드 재생
    }
    public void Stop()
    {
        source.Stop(); // 사운드 정지
    }
    public void SetLoop()
    {
        source.loop = true; 
    }
    public void SetLoopCancel()
    {
        source.loop = false; 
    }
}

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance; // 싱글톤 패턴을 위한 변수 // Scene 이동 간에도 AudioManager가 파괴되지 않도록 하기 위해 static으로 설정

    [SerializeField] // custom class 를 인스펙터에서 편집할 수 있도록 설정
    public Sound[] sounds; // 사운드 리스트

    void Awake() {
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i=0; i<sounds.Length; i++)
        {
            GameObject soundObject = new GameObject("Sound_" + sounds[i].name); // 사운드 오브젝트 생성
            sounds[i].SetSource(soundObject.AddComponent<AudioSource>()); // 사운드 오브젝트에 AudioSource 컴포넌트 추가
            soundObject.transform.SetParent(this.transform); // 사운드 오브젝트를 AudioManager의 자식으로 설정
        }
    }

    public void Play(string _name)
    {
        for (int i=0; i<sounds.Length; i++)
        {
            if(_name == sounds[i].name) // 사운드 이름이 같으면
            {
                sounds[i].source.Play(); // 사운드 재생
                return; // 함수 종료
            }
        }
    }

    public void Stop(string _name)
    {
        for (int i=0; i<sounds.Length; i++)
        {
            if(_name == sounds[i].name) // 사운드 이름이 같으면
            {
                sounds[i].source.Stop(); // 사운드 재생
                return; // 함수 종료
            }
        }
    }

    public void SetLoop(string _name)
    {
        for (int i=0; i<sounds.Length; i++)
        {
            if(_name == sounds[i].name) // 사운드 이름이 같으면
            {
                sounds[i].SetLoop(); // 사운드 재생
                return; // 함수 종료
            }
        }
    }

    public void SetLoopCancel(string _name)
    {
        for (int i=0; i<sounds.Length; i++)
        {
            if(_name == sounds[i].name) // 사운드 이름이 같으면
            {
                sounds[i].SetLoopCancel(); // 사운드 재생
                return; // 함수 종료
            }
        }
    }

    public void SetVolume(string _name, float _Volume)
    {
        for (int i=0; i<sounds.Length; i++)
        {
            if(_name == sounds[i].name) // 사운드 이름이 같으면
            {
                sounds[i].Volume = _Volume; // 사운드 볼륨을 설정
                sounds[i].SetVolume(); // 사운드 볼륨을 설정
                return; // 함수 종료
            }
        }
    }
}
