using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Choice
{
    [TextArea(1, 2)]
    public string[] sentences;        // ��ȭ ����(���� ��)
    public Sprite[] sprites;          // �� ���帶�� ����
    public Sprite[] dialogueWindows;

    [TextArea(1, 2)]
    public string question;           // ���� 1�� (���� ���� �� ����)
    public string[] answers;          // �亯 ���� ��
}
