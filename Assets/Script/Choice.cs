using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Choice
{
    [TextArea(1, 2)]
    public string[] sentences;        // 대화 문장(여러 개)
    public Sprite[] sprites;          // 각 문장마다 대응
    public Sprite[] dialogueWindows;

    [TextArea(1, 2)]
    public string question;           // 질문 1개 (문장 여러 줄 가능)
    public string[] answers;          // 답변 여러 개
}
