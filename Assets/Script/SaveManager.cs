using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

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

    private PlayerManager playerManager;
    private bool prevIsOpen = false; // 이전 isOpen 상태 저장용

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        savePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
        savePanel.SetActive(false);
        saveNLoad = FindFirstObjectByType<SaveNLoad>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerManager = playerObj.GetComponent<PlayerManager>();
    }

    void Update()
    {
        string sceneName = gameObject.scene.name;

        if (sceneName == "Start")
        {
            isStartMenu = true;
            isOpen = true;
            UpdateSlots();
            HighlightSlot(currentIndex);
        }

        // Z키 눌렀을 때 세이브창 열기/닫기 토글 (저장지점 근처일 때만)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isSavePoint)
            {
                isOpen = !isOpen;
                savePanel.SetActive(isOpen);
            }
        }

        // isOpen 상태가 이전 상태와 다를 때만 처리
        if (isOpen != prevIsOpen)
        {
            audioManager.Play(openSound);

            if (isOpen)
            {
                UpdateSlots();
                HighlightSlot(currentIndex);
                if (playerManager != null)
                    playerManager.canMove = false;
            }
            else
            {
                if (playerManager != null)
                    playerManager.canMove = true;
            }

            prevIsOpen = isOpen;
        }

        // 팝업창이 활성화되어 있으면 입력 무시
        if (!isOpen || PopupManager.instance == null || PopupManager.instance.IsPopupActive()) return;

        // 방향키 입력 처리
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCursor(-1);
            audioManager.Play(keySound);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCursor(1);
            audioManager.Play(keySound);
        }

        // 스페이스키로 저장 (게임 중간에서만)
        if (Input.GetKeyDown(KeyCode.Space) && !isStartMenu)
        {
            string path = Application.persistentDataPath + $"/SaveFile_{currentIndex}.dat";
            audioManager.Play(enterSound);

            if (!File.Exists(path))
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
            else
            {
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

        // 엔터키로 로드 (시작 메뉴 혹은 메뉴에서만)
        if (Input.GetKeyDown(KeyCode.Return) && (isStartMenu || isMenu))
        {
            string path = Application.persistentDataPath + $"/SaveFile_{currentIndex}.dat";

            if (File.Exists(path))
            {
                audioManager.Play(enterSound);
                PopupManager.instance.ShowChoicePopup(
                    "불러오시겠습니까?",
                    () =>
                    {
                        saveNLoad.CallLoad(currentIndex);
                    },
                    () =>
                    {
                        Debug.Log("불러오기 취소");
                    }
                );
            }
            else
            {
                audioManager.Play(beepSound);
                PopupManager.instance.ShowPopup("저장 파일이 존재하지 않습니다");
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
                SaveNLoad.Data data = LoadSaveDataFromFile(path);
                slots[i].SetSaveInfo(data.mapName, data.saveDate, data.saveTime, data.sceneName);
            }
            else
            {
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

    void HighlightSlot(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            bool isSelected = (i == index);
            slots[i].SetHighlight(isSelected);
        }
    }

    void MoveCursor(int direction)
    {
        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = slots.Length - 1;
        else if (currentIndex >= slots.Length)
            currentIndex = 0;

        HighlightSlot(currentIndex);
    }
    public bool IsSaveActive() //팝업이 떠있는지 외부에서 확인할 수 있도록 함
    {
        return savePanel.activeSelf;
    }
}