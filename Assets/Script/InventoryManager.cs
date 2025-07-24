using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // 중복 방지
    }

    public AudioManager audioManager; // 오디오 매니저 직접 연결
    public string keySound;
    public string enterSound;
    public string openSound;
    public string beepSound;

    public GameObject inventoryPanel;
    public InventorySlot[] slots; // 슬롯 배열
    public Item[] items; // 아이템 데이터 배열 (슬롯에 들어갈 아이템 정보들)
    private int currentIndex = 0; // 현재 선택된 슬롯 인덱스
    private bool isOpen = false; // 인벤토리 열림 상태

    void Start()
    {
        inventoryPanel.SetActive(false); // 시작 시 인벤토리 패널 비활성화
    }

    void Update()
    {
        // 대화 중일 때 인벤토리 열기 시도 제한: 효과음+팝업창
        if (DialogueManager.instance != null && DialogueManager.instance.talking)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                audioManager.Play(beepSound);
                PopupManager.instance.ShowPopup("대화 중에는 인벤토리를 열 수 없습니다.");
            }
            return; // 대화 중이면 더 이상 진행 X
        }

        // X 키를 눌렀을 때 인벤토리 열고 닫기 토글
        if (Input.GetKeyDown(KeyCode.X))
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive(isOpen);

            if (isOpen)
            {
                audioManager.Play(openSound);
                UpdateSlots(); // 인벤토리 열렸을 때 슬롯에 아이템 정보 갱신
                HighlightSlot(currentIndex); // 현재 인덱스에 해당하는 슬롯에만 강조 표시

                GameObject.FindWithTag("Player").GetComponent<PlayerManager>().canMove = false; //이동 제한
            }
            else
            {
                audioManager.Play(openSound);
                GameObject.FindWithTag("Player").GetComponent<PlayerManager>().canMove = true;
            }
        }

        // 방향키 동작 우선순위: 팝업창>인벤토리>이동
        if (!isOpen || PopupManager.instance.IsPopupActive()) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCursor(-1); // 왼쪽으로 이동
            audioManager.Play(keySound);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCursor(1); // 오른쪽으로 이동
            audioManager.Play(keySound);
        }

        if (Input.GetKeyDown(KeyCode.Z)) //아이템 사용
        {
            Item selectedItem = items[currentIndex];

            if (selectedItem.isObtained)
            {
                audioManager.Play(enterSound);
                PopupManager.instance.ShowChoicePopup(
                    "정말 사용하시겠습니까?",
                    () => {
                        UseItem(selectedItem); // 예
                    },
                    () => {
                        Debug.Log("사용 취소");
                    }
                );
            }
            else
            {
                audioManager.Play(beepSound);
                PopupManager.instance.ShowPopup("아직 획득하지 못한 아이템입니다");
            }
        }
    }

    void UseItem(Item item)
    {
        Debug.Log($"[아이템 사용] {item.itemName}을(를) 사용했습니다!");

        // 아이템 사용 로직
        if (item.itemName == "조리실 사용 규칙" && HiddenNoteEvent.current != null && HiddenNoteEvent.current.eventType == HiddenEventType.Sterilizer)
        {
            HiddenNoteEvent.current.TriggerHiddenNoteEvent();
        }

        // 소모성 아이템일 경우 사용 후 제거
        if (item.itemType == ItemType.Consumable)
        {
            item.isObtained = false;
            items[currentIndex] = item; // 배열 갱신
            UpdateSlots(); // 슬롯 갱신
        }

        isOpen = false;
        inventoryPanel.SetActive(false);
        GameObject.FindWithTag("Player").GetComponent<PlayerManager>().canMove = true;
    }


    public void UpdateSlots() // 모든 슬롯에 아이템 정보 적용
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetItem(items[i]);
        }
    }

    // 아이템 존재 여부 확인용 함수
    public bool HasItem(string itemName)
    {
        foreach (var item in items)
        {
            if (item != null && item.itemName == itemName && item.isObtained)
            {
                return true;
            }
        }
        return false;
    }

    public void ReplaceItem(string oldItemName, Item newItem)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemName == oldItemName)
            {
                items[i] = newItem;
                newItem.isObtained = true;
                UpdateSlots();
                Debug.Log($"[인벤토리] {oldItemName} → {newItem.itemName}으로 교체 완료");
                return;
            }
        }

        Debug.LogWarning("[인벤토리] 교체할 아이템을 찾지 못함: " + oldItemName);
    }

    void HighlightSlot(int index) // 현재 선택된 슬롯만 Outline 활성화
    {
        for (int i = 0; i < slots.Length; i++)
        {
            bool isSelected = (i == index);
            slots[i].SetHighlight(isSelected);
        }
    }

    void MoveCursor(int direction) // 방향키 입력으로 슬롯 선택 이동
    {
        currentIndex += direction;

        // 커서가 배열 범위를 넘으면 순환되도록 처리
        if (currentIndex < 0)
            currentIndex = slots.Length - 1;
        else if (currentIndex >= slots.Length)
            currentIndex = 0;

        HighlightSlot(currentIndex);
    }
}
