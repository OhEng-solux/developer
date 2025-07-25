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

        public string saveDate;   
        public string saveTime;

    }

    private PlayerManager thePlayer;
    private Vector3 playerPositionToLoad;
    public Data data;

    private Vector3 vector;


    public void CallSave(int slotIndex)//����
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

        System.DateTime now = System.DateTime.Now;
        data.saveDate = now.ToString("yyyy-MM-dd");
        data.saveTime = now.ToString("HH:mm:ss");

        //������ ������ ���� �����ϵ��� ���� ���� ����
        BinaryFormatter bf = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, $"SaveFile_{slotIndex}.dat");
        FileStream file = File.Create(path);

        bf.Serialize(file, data);//����ȭ
        file.Close();//���� �������� �Ϸ�

        Debug.Log(Application.dataPath + " �� ��ġ�� �����߽��ϴ�.");
    }


    public void CallLoad(int slotIndex)//�ҷ�����, ���̺��� ���� ����
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, $"SaveFile_{slotIndex}.dat");
        FileStream file = File.Open(path, FileMode.Open);

        if (File.Exists(path))//���� ����� �ε�
        {
            data = (Data)bf.Deserialize(file);

            thePlayer = FindFirstObjectByType<PlayerManager>();

            thePlayer.currentMapName = data.mapName;
            thePlayer.currentSceneName = data.sceneName;
            playerPositionToLoad = new Vector3(data.playerX, data.playerY, data.playerZ);
            
            Debug.Log($"�ε���..??");
            
            Debug.Log($"�ε��� �� �̸�: {data.sceneName}");
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(data.sceneName);

        }
        else
        {
            Debug.Log("���� ���̺� ������ �����ϴ�");
            return;
        }
        file.Close();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� �ε� �Ϸ� �� ȣ���
        if (scene.name == data.sceneName)
        {
            PlayerManager thePlayer = FindObjectOfType<PlayerManager>();
            if (thePlayer != null)
            {
                thePlayer.currentMapName = data.mapName;
                thePlayer.currentSceneName = data.sceneName;
                thePlayer.transform.position = playerPositionToLoad;
            }

            GameManager theGM = FindObjectOfType<GameManager>();
            if (theGM != null)
            {
                theGM.LoadStart();
            }
        }

        // �̺�Ʈ ���� (�ߺ� ���� ����)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
    
