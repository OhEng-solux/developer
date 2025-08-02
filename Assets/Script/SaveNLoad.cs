using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

using static SaveNLoad;


public class SaveNLoad : MonoBehaviour
{
    public static SaveNLoad instance;
    [System.Serializable]//직렬화
    public class Data//모든 세이브 데이터
    {
        public int clueCnt=0;//단서 개수
        public int DialoguCnt=0;//대화 횟수
        public bool[] cluesRead = new bool[4];
        public static bool isLoadingDone = false;

        public float playerX;//직렬화 벡터 사용 불가
        public float playerY;//직렬화 벡터 사용 불가
        public float playerZ;//플레이어 위치 저장

        public List<string> playerItemNames;
        public List<int> playerEquipItem;//장착 아이템

        public string mapName;
        public string sceneName;
        public string characterName;

        public string saveDate;   
        public string saveTime;
        public string targetName="thePlayer";

    }

    private PlayerManager thePlayer;
    private ClueManager theClue;
    private ClueTrigger theCT;
    private DialogueProgressManager theDiaPM;
    private Vector3 playerPositionToLoad;
    public Data data;

    private Vector3 vector;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    public void CallSave(int slotIndex)//저장
    {
        data = new Data();

        thePlayer = FindFirstObjectByType<PlayerManager>();
        InventoryManager theInventory = FindFirstObjectByType<InventoryManager>();
        theClue= FindFirstObjectByType<ClueManager>();
        theCT= FindFirstObjectByType<ClueTrigger>();
        theDiaPM = FindFirstObjectByType<DialogueProgressManager>();

        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        data.characterName = thePlayer.characterName;
        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;

        if (theCT != null)
        {
            data.cluesRead = new bool[ClueTrigger.viewed.Length];
            ClueTrigger.viewed.CopyTo(data.cluesRead, 0);
        }
        if (theClue != null) { 
            data.clueCnt= theClue.clueCount;//단서 개수
        }
        if (theDiaPM != null)
        {
            data.DialoguCnt = theDiaPM.dialogueCount;//대화 횟수
        }
        Debug.Log("기초 데이터 성공");

        if (theInventory == null)
        {
            Debug.LogError("InventoryManager 인스턴스를 찾지 못했습니다.");
        }
        else
        {
            data.playerItemNames = new List<string>();
            foreach (var item in theInventory.items)
            {
                // 획득한 아이템만 저장하거나, 아이템 이름 자체를 모두 저장
                if (item.isObtained)
                    data.playerItemNames.Add(item.itemName);
                else
                    data.playerItemNames.Add("");  // 빈칸으로 빈 슬롯 표현
            }
        }

        System.DateTime now = System.DateTime.Now;
        data.saveDate = now.ToString("yyyy-MM-dd");
        data.saveTime = now.ToString("HH:mm:ss");

        //게임이 꺼져도 저장 가능하도록 물리 파일 생성
        BinaryFormatter bf = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, $"SaveFile_{slotIndex}.dat");
        FileStream file = File.Create(path);

        bf.Serialize(file, data);//직렬화
        file.Close();//파일 내보내기 완료

        Debug.Log(Application.dataPath + " 의 위치에 저장했습니다.");
    }


    public void CallLoad(int slotIndex)//불러오기, 세이브의 역순 진행
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, $"SaveFile_{slotIndex}.dat");
        FileStream file = File.Open(path, FileMode.Open);

        if (File.Exists(path))//파일 존재시 로드
        {
            data = (Data)bf.Deserialize(file);
                       
            Debug.Log($"로드할 씬 이름: {data.sceneName}");
            playerPositionToLoad = new Vector3(data.playerX, data.playerY, data.playerZ);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(data.sceneName);
        }
        else
        {
            Debug.Log("저장 세이브 파일이 없습니다");
            return;
        }
        file.Close();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 완료 후 호출됨
        if (scene.name == data.sceneName)
        {
            CameraManager theCam = FindFirstObjectByType<CameraManager>();
            PlayerManager thePlayer = FindFirstObjectByType<PlayerManager>();
            InventoryManager theInventory = FindFirstObjectByType<InventoryManager>();
            theDiaPM = FindFirstObjectByType<DialogueProgressManager>();
            theClue = FindFirstObjectByType<ClueManager>();
            theCT = FindFirstObjectByType<ClueTrigger>();

            if (thePlayer != null)
            {
                thePlayer.currentMapName = data.mapName;
                thePlayer.currentSceneName = data.sceneName;
                
                if (thePlayer.currentMapName == "Basement")
                {
                    playerPositionToLoad = new Vector3(data.playerX +2, data.playerY, data.playerZ);
                    StartCoroutine(DisableClueTriggersNextFrame());
                }
                thePlayer.transform.position = playerPositionToLoad;
            }
            Debug.Log("OnSceneLoaded");

            GameManager theGM = FindFirstObjectByType<GameManager>();
            if (theGM != null)
            {
                theGM.LoadStart();
            }

            if (theCT != null)
            {
                Array.Copy(data.cluesRead, ClueTrigger.viewed, data.cluesRead.Length);
            }

            if (theClue != null)
            {
                theClue.clueCount = data.clueCnt;//단서 개수
            }

            if (theDiaPM != null)
            {
                theDiaPM.dialogueCount = data.DialoguCnt;//대화 횟수
            }

            if (theInventory != null)
            {
                // 모든 칸을 null로 초기화
                Array.Clear(theInventory.items, 0, theInventory.items.Length);

                // 세이브 데이터 개수만큼 반복 (슬롯 개수 초과 방지)
                int count = Mathf.Min(theInventory.items.Length, data.playerItemNames.Count);

                for (int i = 0; i < count; i++)
                {
                    string itemName = data.playerItemNames[i];

                    if (!string.IsNullOrEmpty(itemName))
                    {
                        Item loaded = Resources.Load<Item>($"Items/{itemName}");
                        if (loaded != null)
                        {
                            Item instance = Instantiate(loaded);
                            instance.isObtained = true;
                            theInventory.items[i] = instance;
                        }
                        else
                        {
                            // 아이템 에셋 못 찾음
                            theInventory.items[i] = null;
                        }
                    }
                    else
                    {
                        // 저장당시 빈 슬롯
                        theInventory.items[i] = null;
                    }
                }
                // 만약 인벤토리가 저장 당시보다 슬롯이 더 많다면, 남는 칸도 모두 null 상태
            }
        }

        Array.Copy(data.cluesRead, ClueTrigger.viewed, data.cluesRead.Length);


        // 이벤트 해제 (중복 실행 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private IEnumerator DisableClueTriggersNextFrame()
    {
        yield return null; // 한 프레임 대기 (모든 Awake/Start 실행 보장)

        ClueTrigger[] triggers = FindObjectsOfType<ClueTrigger>();
        foreach (var trigger in triggers)
        {
            BoxCollider2D col = trigger.GetComponent<BoxCollider2D>();
            if (col != null)
                col.enabled = false;
        }
        
    }
}
    
