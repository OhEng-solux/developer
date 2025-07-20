/*
using System.Collection;
using System.Collection.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatter.Binary;
using UnityEngine.SceneManagment;


public class SaveNLoad : MonoBehaviour
{
    [System.Serializable]//����ȭ
    public class Data//��� ���̺� ������
    {
        public float playerX;//����ȭ ���� ��� �Ұ�
        public float playerY;//����ȭ ���� ��� �Ұ�
        public float playerZ;//�÷��̾� ��ġ ����

        //public List<int> playerItemInventory;//�ڷ��� ���� ����ȭ �Ұ�
       // public List<int> playerItemInventoryCount;
        public List<int> playerEquipItem;//���� ������

        public string mapName;
        public string sceneName;

        //�����ͺ��̽� ���� �� 
        public List<bool> swList;
        public List<string> swNameList;
        public List<string> varNameList;
        public List<string> varNumberList;

    }

    private PlayerManager thePlayer;
    //private PlayerStat thrPlayerStat;�÷��̾� ����
    private DatabaseManager theDataBase;
    //private Inventory theInven;
    private Equipment theEquip;

    public Data data;

    private Vector3 vector;


    public void CallSave()//����
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

        Debug.Log("���� ������ ����");

        for (int i = 0; i < theDataBase.var_name.Length; i++)//����
        {
            data.varNameList.Add(theDataBase.var_name[i]);
            data.varNumberList.Add(theDataBase.var[i]);
        }
        for (int i = 0; i < theDataBase.switch_name.Length; i++)//switch
        {
            data.switchNameList.Add(theDataBase.switch_name[i]);
            data.swList.Add(theDataBase.switches[i]);
        }
        //�κ��丮 �ڵ� �Ϸ� �� Ȱ��ȭ
        /*List<Item> itemList = theInven.SaveItem(); 
        for(int i=0;i<itemList.Count;i++)
        {
            Debug.Log("�κ��丮 ������ ���� �Ϸ�:"+itemList[i].itemID);
            data.playerItemInventory.Add(itemList[i].itemID);
            data.playerItemInventoryCOunt.Add(itemList[i].itemCount);
        }
        */
/*
        for(int i=0; i < theEquip.equipItemList.Length; i++)//���� ������
        {
            data.playerEquipmentItem.Add(theEquip.equipItemList[i].itemID);
            Debug.Log("������ ������ ���� �Ϸ�:" + theEquip.equipItemList[i].itemID);

        }
        //������ ������ ���� �����ϵ��� ���� ���� ����
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/SaveFile.dat");

        bf.Serialize(file, data);//����ȭ
        file.Close();//���� �������� �Ϸ�

        Debug.Load(Application.dataPath + " �� ��ġ�� �����߽��ϴ�.");
    }


    public void CallLoad()//�ҷ�����, ���̺��� ���� ����
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/SaveFile.dat",FileMode.Open);

        if(file!=null && file.Length > 0)//���� ����� �ε�
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
                        Debug.Log("���� ������ �ε�:" + theEquip.equipItemList[i].itemID);
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
                        Debug.Log("�κ� ������ �ε�:" + theDatabase.itemList[x].itemID);
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
            Debug.Log("���� ���̺� ������ �����ϴ�");
        }
        file.Close();
    }
    
}
*/