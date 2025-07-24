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
        switch (eventType)
        {
            case HiddenEventType.Refrigerator:
                imageUI.SetActive(true); // 쪽지 이미지 보여주기

                // 아이템 획득 처리
                if (itemToAdd != null)
                {
                    itemToAdd.isObtained = true;
                    Debug.Log("[냉장고] 아이템 획득: " + itemToAdd.itemName);
                    InventoryManager.instance.UpdateSlots(); // UI 갱신
                }

                DialogueManager.instance.ShowDialogue(endDialogue); // 대사 출력 시작
                StartCoroutine(HideImageWhenDialogueEnds()); // 끝날 때 이미지도 꺼짐
                break;

            case HiddenEventType.Sterilizer:
                if (InventoryManager.instance.HasItem(itemToReplace.itemName))
                {
                    InventoryManager.instance.ReplaceItem(itemToReplace.itemName, itemToAdd); // 아이템 교체
                    imageUI.SetActive(true); // 쪽지 이미지 보여주기
                    DialogueManager.instance.ShowDialogue(endDialogue); // 대사 출력                    
                    StartCoroutine(HideImageWhenDialogueEnds()); // 대사 끝나면 이미지 사라지게
                }
                else
                {
                    Debug.Log("[살균기] 아이템이 없어 교체 불가");
                    PopupManager.instance.ShowPopup("쪽지가 없다면 아무 일도 일어나지 않는다...");
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDone) return; // 일회성 이벤트로 제한

        if (col.CompareTag("Player"))
        {
            isPlayerInRange = true;
            isZKeyReady = false;
            DialogueManager.instance.ShowDialogue(preDialogue);
            isZKeyReady = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerInRange = false;
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
}
