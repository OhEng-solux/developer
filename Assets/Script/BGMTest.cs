using System.Collections;
using UnityEngine;

public class BGMTest : MonoBehaviour
{
    private BGMManager BGM;
    public int playMusicTrack;

    void Start()
    {
        BGM = BGMManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(SwitchAndDisable());
    }

    IEnumerator SwitchAndDisable()
    {
        yield return StartCoroutine(BGM.SwitchBGMWithFade(playMusicTrack));
        this.gameObject.SetActive(false);
    }
}
