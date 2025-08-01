using UnityEngine;

[RequireComponent(typeof(TransferMap))]
public class TransferSoundTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip transferSound;
    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Awake()
    {
        // 자기 자신 혹은 상위 오브젝트에서 AudioSource 찾기
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    // 외부에서 호출
    public void PlayTransferSoundOnce()
    {
        if (!hasPlayed && transferSound != null)
        {
            audioSource.PlayOneShot(transferSound);
            hasPlayed = true;
        }
    }

    // 필요 시 다른 곳에서도 다시 초기화 가능
    public void ResetSoundPlayedFlag()
    {
        hasPlayed = false;
    }
}
