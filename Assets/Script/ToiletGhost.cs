using UnityEngine;
using System.Collections;

public class ToiletGhost : MonoBehaviour
{
    public GameObject face;
    public GameObject left_1;
    public GameObject right_1;
    public GameObject left_2;
    public GameObject right_2;
    public GameObject left_3;
    public GameObject right_3;

    public float delayBetweenParts = 0.0001f;
    private bool hasTriggered = false;

    void Start()
    {
        face.SetActive(false); 
        left_1.SetActive(false);
        right_1.SetActive(false);
        left_2.SetActive(false);
        right_2.SetActive(false);
        left_3.SetActive(false);
        right_3.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return; // 🔒 이미 실행됐으면 무시

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true; // ✅ 여기서 먼저 막아버림

            StartCoroutine(ActivateFace());
            StartCoroutine(ActivatePartsSequentially());
        }
    }
    private IEnumerator ActivateFace()
    {
        yield return new WaitForSeconds(1f);
        face.SetActive(true);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ActivatePartsSequentially()
    {
        
        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("mirror_footstep");

        right_1.SetActive(true);
        yield return new WaitForSeconds(0.145f);

        left_1.SetActive(true);
        yield return new WaitForSeconds(0.145f);

        right_2.SetActive(true);
        yield return new WaitForSeconds(0.145f);

        left_2.SetActive(true);
        yield return new WaitForSeconds(0.145f);

        right_3.SetActive(true);
        yield return new WaitForSeconds(0.145f);

        left_3.SetActive(true);
    }
}
