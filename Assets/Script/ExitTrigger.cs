using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    public enum ExitType { FrontDoor, SideDoor }
    public ExitType exitType;

    private bool playerInRange = false;

    [TextArea]
    public string warningText;
    public Sprite warningWindow;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            TryExit();
        }
    }

    private void TryExit()
    {
        if (ChaseTriggerManager.instance == null)
        {
            Debug.LogWarning("ChaseTriggerManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        if (!ChaseTriggerManager.instance.isChasing) // 추격 시작 여부 확인
        {
            Debug.Log("아직 탈출할 수 없다. 쫓기고 있지 않다.");
            return;
        }
        
        if (ClueManager.instance == null)
        {
            Debug.LogWarning("ClueManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        bool flowerBroken = ClueManager.instance.flowerBroken;

        switch (exitType)
        {
            case ExitType.FrontDoor:
                if (flowerBroken)
                {
                    SceneManager.LoadScene("Ending_True");
                }
                else
                {
                    if (!string.IsNullOrEmpty(warningText) && DialogueManager.instance != null)
                    {
                        Dialogue dialogue = new Dialogue();
                        dialogue.sentences = new string[] { warningText };
                        dialogue.dialogueWindows = new Sprite[] { warningWindow };
                        dialogue.sprites = new Sprite[] { null };

                        DialogueManager.instance.ShowDialogue(dialogue, false);
                    }
                    else
                    {
                        Debug.Log("꽃병 깨짐 상태가 아님");
                    }
                }
                break;

            case ExitType.SideDoor:
                if (!flowerBroken)
                {
                    SceneManager.LoadScene("Ending_Hidden");
                }
                else
                {
                    if (!string.IsNullOrEmpty(warningText) && DialogueManager.instance != null)
                    {
                        Dialogue dialogue = new Dialogue();
                        dialogue.sentences = new string[] { warningText };
                        dialogue.dialogueWindows = new Sprite[] { warningWindow };
                        dialogue.sprites = new Sprite[] { null };

                        DialogueManager.instance.ShowDialogue(dialogue, false);
                    }
                    else
                    {
                        Debug.Log("꽃병 깨짐 상태 -> 정문 탈출 유도");
                    }
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
