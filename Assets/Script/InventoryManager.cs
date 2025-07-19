using UnityEngine;

public class InventoryManager : MonoBehaviour
{
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
        // 대화 중일 때 인벤토리 열기 시도 → 막기 + 효과음
        if (DialogueManager.instance != null && DialogueManager.instance.talking)
        {
            if (Input.GetKeyDown(KeyCode.X)) audioManager.Play(beepSound);
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

        // 인벤토리가 열려 있을 때만 방향키 동작
        if (!isOpen) return;

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
