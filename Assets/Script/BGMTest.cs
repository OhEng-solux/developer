using System.Collections;
using UnityEngine;

public class BGMTest : MonoBehaviour
{
    BGMManager BGM;
    public int playMusicTrack;

    void Start()
    {
        BGM = FindObjectOfType<BGMManager>();
        Debug.Log("BGMManager 찾은 결과: " + (BGM == null ? "null" : "OK"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Fade 전환
        StartCoroutine(ChangeBGMWithFade());
        this.gameObject.SetActive(false);
    }

    IEnumerator ChangeBGMWithFade()
    {
        BGM.FadeOutMusic();
        yield return new WaitForSeconds(0.5f); // 페이드아웃(1초) 기다린 뒤
        BGM.Play(playMusicTrack);
        BGM.FadeInMusic();
    }
}
