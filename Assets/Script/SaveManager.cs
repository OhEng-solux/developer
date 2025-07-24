using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
    private string keySound = "type_Sound";
    private string enterSound = "enter_Sound";
    private string openSound = "ok_Sound";
    private string beepSound = "beep_Sound";

    public GameObject savePanel;
    public SaveSlot[] slots; // 슬롯 배열


    private int currentIndex = 0; // 현재 선택된 슬롯 인덱스
    private bool isOpen = false; // 열림 상태
    private bool isSavePoint = false;
    private bool isStartMenu = false;
    private bool isMenu = false;
    private AudioManager audioManager;
    private SaveNLoad saveNLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // "SavePoint" 태그를 붙인 물체와 닿았을 때
        if (other.CompareTag("SavePoint"))
        {
            isSavePoint = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SavePoint"))
        {
            isSavePoint = false;
        }
    }

    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        savePanel.SetActive(false); // 시작 시 패널 비활성화r.so
        saveNLoad = FindFirstObjectByType<SaveNLoad>();

    }

    void Update()
    {
        string sceneName = gameObject.scene.name;
        if (sceneName=="Start")
        {
            isStartMenu = true;
            isOpen=true;
            UpdateSlots();
            HighlightSlot(currentIndex);
        }

        // 눌렀을 때 세이브창 열고 닫기 토글
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isSavePoint) // 근처일 때만 토글 허용
            {
                isOpen = !isOpen;
                savePanel.SetActive(isOpen);

                if (isOpen)
                {
                    audioManager.Play(openSound);
                    UpdateSlots();
                    HighlightSlot(currentIndex);
                    GameObject.FindWithTag("Player").GetComponent<PlayerManager>().canMove = false;
                }
                else
                {
                    audioManager.Play(openSound);
                    GameObject.FindWithTag("Player").GetComponent<PlayerManager>().canMove = true;
                }
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMenu = true;
            isOpen = true;
            UpdateSlots();
            HighlightSlot(currentIndex);
        }


        // 방향키 동작 우선순위: 팝업창>인벤토리>이동
        if (!isOpen || PopupManager.instance.IsPopupActive()) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCursor(-1); // 위쪽으로 이동
            audioManager.Play(keySound);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCursor(1); // 아래쪽으로 이동
            audioManager.Play(keySound);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isStartMenu) //세이브는 게임 중간에서만
        {
            string path = Application.persistentDataPath + $"/SaveFile_{currentIndex}.dat";
            audioManager.Play(enterSound);

            if (!System.IO.File.Exists(path))
            {
                Debug.Log("저장 ?");
                PopupManager.instance.ShowChoicePopup(
                    "저장하시겠습니까?",
                    () =>
                    {
                        saveNLoad.CallSave(currentIndex);
                        UpdateSlots();
                    },
                    () =>
                    {
                        Debug.Log("저장 취소");
                    }
                );
            }
            else {
                Debug.Log("덮어 ?");
                PopupManager.instance.ShowChoicePopup(

                    "덮어쓰시겠습니까?",
                    () =>
                    {
                        saveNLoad.CallSave(currentIndex);
                        UpdateSlots();
                    },
                    () =>
                    {
                        Debug.Log("덮어쓰기 취소");
                    }
                );
            }

        }

        if (Input.GetKeyDown(KeyCode.Return) && (isStartMenu|| isMenu)) //로드는 메인메뉴에서만
        {
            string path = Application.persistentDataPath + $"/SaveFile_{currentIndex}.dat";

            if (System.IO.File.Exists(path))
            {
                audioManager.Play(enterSound);
                PopupManager.instance.ShowChoicePopup(
                    "불러오시겠습니까?",
                    () => {
                        saveNLoad.CallLoad(currentIndex);
                    },
                    () => {
                        Debug.Log("불러오기 취소");
                    }
                );
            }

            else
            {
                audioManager.Play(beepSound);
                PopupManager.instance.ShowPopup("저장 파일이 존재합니다");
            }
        }
    }

    public void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            string path = Application.persistentDataPath + $"/SaveFile_{i}.dat";

            if (File.Exists(path))
            {
                // 저장 파일이 존재하면 파일에서 메타 정보만 읽어와 UI에 전달
                SaveNLoad.Data data = LoadSaveDataFromFile(path);
                Debug.Log("업데이트중");
                slots[i].SetSaveInfo(data.mapName, data.saveDate, data.saveTime,data.sceneName);
            }
            else
            {
                // 저장 파일이 없으면 슬롯을 비워두거나 “저장 없음” 표시
                slots[i].SetEmpty();
            }
        }
    }

    private SaveNLoad.Data LoadSaveDataFromFile(string path)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Open(path, FileMode.Open))
        {
            return (SaveNLoad.Data)bf.Deserialize(file);
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
