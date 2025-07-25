using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    [TextArea(1, 2)]
    public string[] sentences;
    public Sprite[] sprites;
    public Sprite[] dialogueWindows;

    [TextArea(1, 2)] // 대화 문장 길이 제한
    public string[] blueSentences; // 파란색 대화 문장 배열
    [TextArea(1, 2)] // 대화 문장 길이 제한
    public string[] yellowSentences; // 파란색 대화 문장 배열
}

