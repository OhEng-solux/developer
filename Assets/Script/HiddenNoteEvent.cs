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

    private bool isPlayerInRange = false;
    private bool isZKeyReady = false;
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

        imageUI.SetActive(true); // 쪽지 이미지 보여주기

        if (itemToAdd != null)
        {
            itemToAdd.isObtained = true;
            Debug.Log("[냉장고] 아이템 획득: " + itemToAdd.itemName);
            InventoryManager.instance.UpdateSlots(); // UI 갱신
        }

        DialogueManager.instance.ShowDialogue(endDialogue);
        StartCoroutine(HideImageWhenDialogueEnds());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDone) return; // 일회성 이벤트로 제한

        if (col.CompareTag("Player"))
        {
            isPlayerInRange = true;
            isZKeyReady = false;
            current = this;
            DialogueManager.instance.ShowDialogue(preDialogue);
            
            if (eventType == HiddenEventType.Refrigerator)
                isZKeyReady = true;
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

        if (InventoryManager.instance.HasItem(itemToReplace.itemName))
        {
            InventoryManager.instance.ReplaceItem(itemToReplace.itemName, itemToAdd);
            imageUI.SetActive(true);
            DialogueManager.instance.ShowDialogue(endDialogue);
            StartCoroutine(HideImageWhenDialogueEnds());
        }
        else
        {
            Debug.Log("[살균기] 아이템이 없어 교체 불가");
            PopupManager.instance.ShowPopup("쪽지가 없다면 아무 일도 일어나지 않는다...");
        }
    }

}
