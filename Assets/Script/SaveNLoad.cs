/*
using System.Collection;
using System.Collection.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatter.Binary;
using UnityEngine.SceneManagment;


public class SaveNLoad : MonoBehaviour
{
    [System.Serializable]//직렬화
    public class Data//모든 세이브 데이터
    {
        public float playerX;//직렬화 벡터 사용 불가
        public float playerY;//직렬화 벡터 사용 불가
        public float playerZ;//플레이어 위치 저장

        //public List<int> playerItemInventory;//자료형 제외 직렬화 불가
       // public List<int> playerItemInventoryCount;
        public List<int> playerEquipItem;//장착 아이템

        public string mapName;
        public string sceneName;

        //데이터베이스 선언 속 
        public List<bool> swList;
        public List<string> swNameList;
        public List<string> varNameList;
        public List<string> varNumberList;

    }

    private PlayerManager thePlayer;
    //private PlayerStat thrPlayerStat;플레이어 스탯
    private DatabaseManager theDataBase;
    //private Inventory theInven;
    private Equipment theEquip;

    public Data data;

    private Vector3 vector;


    public void CallSave()//저장
    {
        theDataBase = FindFirstObjectByType<DatabaseManager>();
        thePlayer = FindFirstObjectByType<PlayerManager>();
        theEquip = FindFirstObjectByType<Equipment>();
        //tneInven = FindFirstObjectByType<Inventory>();

        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;

        Debug.Log("기초 데이터 성공");

        for (int i = 0; i < theDataBase.var_name.Length; i++)//변수
        {
            data.varNameList.Add(theDataBase.var_name[i]);
            data.varNumberList.Add(theDataBase.var[i]);
        }
        for (int i = 0; i < theDataBase.switch_name.Length; i++)//switch
        {
            data.switchNameList.Add(theDataBase.switch_name[i]);
            data.swList.Add(theDataBase.switches[i]);
        }
        //인벤토리 코드 완료 후 활성화
        /*List<Item> itemList = theInven.SaveItem(); 
        for(int i=0;i<itemList.Count;i++)
        {
            Debug.Log("인벤토리 아이템 저장 완료:"+itemList[i].itemID);
            data.playerItemInventory.Add(itemList[i].itemID);
            data.playerItemInventoryCOunt.Add(itemList[i].itemCount);
        }
        */
/*
        for(int i=0; i < theEquip.equipItemList.Length; i++)//장착 아이템
        {
            data.playerEquipmentItem.Add(theEquip.equipItemList[i].itemID);
            Debug.Log("장착된 아이템 저장 완료:" + theEquip.equipItemList[i].itemID);

        }
        //게임이 꺼져도 저장 가능하도록 물리 파일 생성
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/SaveFile.dat");

        bf.Serialize(file, data);//직렬화
        file.Close();//파일 내보내기 완료

        Debug.Load(Application.dataPath + " 의 위치에 저장했습니다.");
    }


    public void CallLoad()//불러오기, 세이브의 역순 진행
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/SaveFile.dat",FileMode.Open);

        if(file!=null && file.Length > 0)//파일 존재시 로드
        {
            data = (Data)bf.Deserialize(file);

            theDataBase = FindFirstObjectByType<DatabaseManager>();
            thePlayer = FindFirstObjectByType<PlayerManager>();
            theEquip = FindFirstObjectByType<Equipment>();
            //tneInven = FindFirstObjectByType<Inventory>();

            thePlayer.currentMapName = data.mapName;
            thePlayer.currentSceneName = data.SceneName;

            vertor.Set(data.playerX, data.playerY, data.playerZ);
            thePlayer.tramsform.position = vector;

            theDataBase.var = data.varNumberList.ToArray();
            theDataBase.var_name = data.varNameList.ToArray();
            theDataBase.switches=data.swList.ToArray();
            theDataBase.switch_name=data.swNameList.ToArray();

            for(int i=0; i < theEquip.equipItemList.Length; i++)
            {
                for(int x = 0; x < theDatabase.itemList.Count; x++)
                {
                    if (data.playerEquipItem[i] == theDataBase.itemList[x].itemID)
                    {
                        theEquip.equipItemList[i]=theDataBase.itemList[x];
                        Debug.Log("장착 아이템 로드:" + theEquip.equipItemList[i].itemID);
                        break;
                    }
                }
            }
            /*
            List<Item> itemList = new List<Item>();
            for (int i = 0; i < data.playerItemInventory.Count; i++)
            {
                for (int x = 0; x < data.playerItemInventory.Count; x++)
                {
                    if (data.playerInventoryItem[i] == theDataBase.itemList[x].itemID)
                    {
                        itemList.Add(theDatabase.itemList[x]);
                        Debug.Log("인벤 아이템 로드:" + theDatabase.itemList[x].itemID);
                        break;
                    }
                }
            }
            

            for(int i = 0; i < data.playerItemInventoryCount.Count; i++)
            {
                itemList.itemCount = data.playerItemInventoryCount[i];
            }
            theIncen.Loaditem(itemList);
            */
/*
theEquip.ShowTxT();
           

            SceneManager.LoadScene(data.sceneName);
            
        }
        else
        {
            Debug.Log("저장 세이브 파일이 없습니다");
        }
        file.Close();
    }
    
}
*/