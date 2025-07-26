using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum HiddenEventType { Refrigerator, Sterilizer }

public class HiddenNoteEvent : MonoBehaviour
{
    public HiddenEventType eventType;

    public Dialogue preDialogue;
    public Dialogue endDialogue;

    public Sprite clueImage; // 냉장고용
    public GameObject imageUI; // 이미지 띄우는 UI
    public Item itemToAdd; // 얻을 아이템
    public Item itemToReplace; // 기존 아이템 (살균기용)
    [SerializeField] private GameObject objectToDisable; // 쪽지 습득 시 사라질 오브젝트

    private bool isPlayerInRange = false;
    private bool isZKeyReady = false;
    private bool hasPlayedPreDialogue = false; // predialogue는 한 번만 출력
    private bool isDone = false;

    public static HiddenNoteEvent current; // 현재 범위 안의 이벤트

    void Update()
    {
        if (!isPlayerInRange) return;

        if (Input.GetKeyDown(KeyCode.Z) && isZKeyReady)
        {
            HandleZKeyEvent();
            isZKeyReady = false;
        }
    }

    void HandleZKeyEvent()
    {
        if (eventType != HiddenEventType.Refrigerator) return;

        if (itemToAdd != null && !itemToAdd.isObtained)
        {
            itemToAdd.isObtained = true;
            InventoryManager.instance.AcquireItem(itemToAdd);
            Debug.Log("[냉장고] 아이템 획득: " + itemToAdd.itemName);
        }

        // 오브젝트 비활성화 처리
        if (objectToDisable != null && objectToDisable.activeSelf)
        {
            objectToDisable.SetActive(false);
            Debug.Log("[냉장고] 관련 오브젝트 비활성화됨: " + objectToDisable.name);
        }

        imageUI.SetActive(true); // 쪽지 이미지 보여주기
        DialogueManager.instance.ShowDialogue(endDialogue);
        StartCoroutine(HideImageWhenDialogueEnds());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDone || DialogueManager.instance.talking) return; // 일회성 이벤트로 제한

        if (col.CompareTag("Player"))
        {
            isPlayerInRange = true;
            isZKeyReady = false;
            current = this;
            
            // 냉장고: 무조건 대사 출력 + Z키 허용
            if (eventType == HiddenEventType.Refrigerator)
            {
                if (!hasPlayedPreDialogue)
                {
                    DialogueManager.instance.ShowDialogue(preDialogue);
                    hasPlayedPreDialogue = true;
                }
                isZKeyReady = true;
            }

            // 살균기: 특정 아이템이 있을 때만 대사 출력 + Z키 허용
            else if (eventType == HiddenEventType.Sterilizer && itemToReplace != null && itemToReplace.isObtained)
            {
                if (!hasPlayedPreDialogue)
                {
                    DialogueManager.instance.ShowDialogue(preDialogue);
                    hasPlayedPreDialogue = true;
                }
                isZKeyReady = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (eventType == HiddenEventType.Sterilizer && current == this)
            current = null;
        }
    }

    private IEnumerator HideImageWhenDialogueEnds()
    {
        // DialogueManager가 대사 중일 때까지 대기
        yield return new WaitUntil(() => !DialogueManager.instance.talking);
        // 대사 끝났으면 이미지 끄기
        imageUI.SetActive(false);
        isDone = true;
    }

    public void TriggerHiddenNoteEvent()
    {
        if (eventType != HiddenEventType.Sterilizer) return;

        if (isDone) return;

        if (itemToReplace != null && itemToReplace.isObtained)
        {
            InventoryManager.instance.ReplaceItem(itemToReplace.itemName, itemToAdd);
            imageUI.SetActive(true);
            DialogueManager.instance.ShowDialogue(endDialogue);
            StartCoroutine(HideImageWhenDialogueEnds());
        }
        else
        {
            PopupManager.instance.ShowPopup("쪽지가 없다면 아무 일도 일어나지 않는다...");
        }
    }

}
