using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    [TextArea(1, 2)] // 대화 문장 길이 제한
    public string[] sentences; //-> 대화 문장 배열
    public Sprite[] sprites;
    public Sprite[] dialogueWindows; //-> 대화창 이미지 배열 // 누구 대화창인지

}
