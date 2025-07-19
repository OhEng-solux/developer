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
            float currentTime = Time.time;
            if (!theDM.talking && currentTime - lastTalkTime > talkCooldown)
            {
                lastTalkTime = currentTime;
                theDM.ShowDialogue(dialogue, false); // 대화는 나오지만 카운트 안 올림

            }
        }
    }
}
