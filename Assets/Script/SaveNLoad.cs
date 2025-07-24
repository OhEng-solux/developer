using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;


public class SaveNLoad : MonoBehaviour
{
    [System.Serializable]//직렬화
    public class Data//모든 세이브 데이터
    {
        public float playerX;//직렬화 벡터 사용 불가
        public float playerY;//직렬화 벡터 사용 불가
        public float playerZ;//플레이어 위치 저장

        public List<string> playerItemNames;
        public List<int> playerEquipItem;//장착 아이템

        public string mapName;
        public string sceneName;

        public string saveDate;   
        public string saveTime;

    }

    private PlayerManager thePlayer;

    public Data data;

    private Vector3 vector;


    public void CallSave(int slotIndex)//저장
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
        InventoryManager theInventory = FindFirstObjectByType<InventoryManager>();

        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;

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
        FileStream file = File.Open(Application.persistentDataPath + "/SaveFile.dat",FileMode.Open);

        if (File.Exists(path))//파일 존재시 로드
        {
            data = (Data)bf.Deserialize(file);

            thePlayer = FindFirstObjectByType<PlayerManager>();

            thePlayer.currentMapName = data.mapName;
            thePlayer.currentSceneName = data.sceneName;

            vector.Set(data.playerX, data.playerY, data.playerZ);
            thePlayer.transform.position = vector;

            GameManager theGM = FindFirstObjectByType<GameManager>();
            theGM.LoadStart();

            SceneManager.LoadScene(data.sceneName);
        }
        else
        {
            Debug.Log("저장 세이브 파일이 없습니다");
            return;
        }
        file.Close();
    }
    
}
