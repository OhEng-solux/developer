using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class TestDialogue_box : MonoBehaviour
{
    [SerializeField]
    public Dialogue dialogue;

    private DialogueManager theDM;
    private BoxCollider2D boxCollider;

    private float talkCooldown = 1f; // 재대화 가능 시간 간격 (초)
    private float lastTalkTime = -10f; // 마지막 대화 시간

    private bool hasTalked = false; // 이미 대화했는지 여부

    void Start()
    {
        theDM = Object.FindAnyObjectByType<DialogueManager>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private string ReplacePlayerName(string original)
    {
        string name = PlayerManager.instance.characterName;
        return original.Replace("$playerName", name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            //  이미 대화한 경우 무시
            if (hasTalked) return;

            float currentTime = Time.time;
            if (!theDM.talking && currentTime - lastTalkTime > talkCooldown)
            {
                lastTalkTime = currentTime;
                hasTalked = true; // 대화했음을 기록
                theDM.ShowDialogue(dialogue, false);
            }
        }
    }
}
