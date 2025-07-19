using UnityEngine;

public class InventoryManager : MonoBehaviour
{
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
        // X 키를 눌렀을 때 인벤토리 열고 닫기 토글
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X key pressed"); // 🔴 디버그 로그 추가
            isOpen = !isOpen;
            inventoryPanel.SetActive(isOpen);

            if (isOpen)
            {
                Debug.Log("Inventory Opened"); // 🔴 디버그 로그 추가
                UpdateSlots(); // 인벤토리 열렸을 때 슬롯에 아이템 정보 갱신
                HighlightSlot(currentIndex); // 현재 인덱스에 해당하는 슬롯에만 강조 표시
            }
        }

        // 인벤토리가 열려 있을 때만 방향키 동작
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveHighlight(-1); // 왼쪽으로 이동
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveHighlight(1); // 오른쪽으로 이동
        }
    }

    void UpdateSlots() // 모든 슬롯에 아이템 정보 적용
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetItem(items[i]);
        }
    }

    void HighlightSlot(int index) // 현재 선택된 슬롯만 Outline 활성화
    {
        for (int i = 0; i < slots.Length; i++)
        {
            bool isSelected = (i == index);
            slots[i].SetHighlight(isSelected);
        }
    }

    void MoveHighlight(int direction) // 방향키 입력으로 슬롯 선택 이동
    {
        currentIndex += direction;

        // 경계 체크: 0 ~ (슬롯 개수 - 1)
        if (currentIndex < 0) currentIndex = 0;
        if (currentIndex >= slots.Length) currentIndex = slots.Length - 1;

        HighlightSlot(currentIndex);
    }
}
