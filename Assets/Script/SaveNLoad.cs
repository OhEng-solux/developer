using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;


public class SaveNLoad : MonoBehaviour
{
    [System.Serializable]//����ȭ
    public class Data//��� ���̺� ������
    {
        public float playerX;//����ȭ ���� ��� �Ұ�
        public float playerY;//����ȭ ���� ��� �Ұ�
        public float playerZ;//�÷��̾� ��ġ ����

        public List<string> playerItemNames;
        public List<int> playerEquipItem;//���� ������

        public string mapName;
        public string sceneName;

    }

    private PlayerManager thePlayer;

    public Data data;

    private Vector3 vector;


    public void CallSave()//����
    {
        thePlayer = FindFirstObjectByType<PlayerManager>();
        InventoryManager theInventory = FindFirstObjectByType<InventoryManager>();

        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;

        Debug.Log("���� ������ ����");

        if (theInventory == null)
        {
            Debug.LogError("InventoryManager �ν��Ͻ��� ã�� ���߽��ϴ�.");
        }
        else
        {
            data.playerItemNames = new List<string>();
            foreach (var item in theInventory.items)
            {
                // ȹ���� �����۸� �����ϰų�, ������ �̸� ��ü�� ��� ����
                if (item.isObtained)
                    data.playerItemNames.Add(item.itemName);
                else
                    data.playerItemNames.Add("");  // ��ĭ���� �� ���� ǥ��
            }
        }
        //������ ������ ���� �����ϵ��� ���� ���� ����
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveFile.dat");

        bf.Serialize(file, data);//����ȭ
        file.Close();//���� �������� �Ϸ�

        Debug.Log(Application.dataPath + " �� ��ġ�� �����߽��ϴ�.");
    }


    public void CallLoad()//�ҷ�����, ���̺��� ���� ����
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/SaveFile.dat",FileMode.Open);


        if (file!=null && file.Length > 0)//���� ����� �ε�
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
            Debug.Log("���� ���̺� ������ �����ϴ�");
        }
        file.Close();
    }
    
}
