using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public List<string> correctAnswer = new List<string> { "��", "��", "��", "��" };
    private List<string> playerInput = new List<string>();

    public GameObject puzzlePanel;
    public Dialogue successDialogue;
    public Dialogue failDialogue;
    private DialogueManager dm;

    public GameObject keyObject; // ���� ������ �Ǵ� �̹���
    public Light spotlight; // ���� ȿ�� (Optional)

    private void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
        keyObject.SetActive(false);
    }

    public void ButtonPressed(string shape)
    {
        playerInput.Add(shape);

        if (playerInput.Count == 4)
        {
            if (IsCorrect())
                StartCoroutine(SuccessSequence());
            else
                StartCoroutine(FailSequence());
        }
    }

    bool IsCorrect()
    {
        for (int i = 0; i < 4; i++)
            if (playerInput[i] != correctAnswer[i]) return false;
        return true;
    }

    IEnumerator SuccessSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;
        dm.ShowDialogue(successDialogue);

        keyObject.SetActive(true); // ���� �����ֱ�
        if (spotlight != null) spotlight.intensity = 0.2f; // ���� ��Ӱ�
        // ���� �κ��丮�� �߰��ϴ� ������ ���⼭ ȣ�� ����
    }

    IEnumerator FailSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;
        dm.ShowDialogue(failDialogue);
        playerInput.Clear(); // �ʱ�ȭ
    }
}
