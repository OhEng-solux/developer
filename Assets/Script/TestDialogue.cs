using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class TestDialogue : MonoBehaviour
{
    [SerializeField]
    public Dialogue dialogue; // Dialogue 스크립트의 인스턴스
    private DialogueManager theDM; // DialogueManager 스크립트의 인스턴스
    private BoxCollider2D boxCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        theDM = Object.FindAnyObjectByType<DialogueManager>(); // DialogueManager 인스턴스를 찾습니다.
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private string ReplacePlayerName(string original)
    {
        string name = PlayerManager.instance.characterName; // 혹은 FindObjectOfType<PlayerManager>().characterName;
        return original.Replace("$playerName", name);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player") // 플레이어가 충돌했을 때
        {
            if (!theDM.talking) // DialogueManager가 대화 중이 아닐 때 -> 대화가 두 번 반복되는 버그 해결
            {
                theDM.ShowDialogue(dialogue);
                boxCollider.enabled = false;
                // DialogueManager의 ShowDialogue 메서드를 호출하여 대화를 시작합니다.
            }

        }
    }
}