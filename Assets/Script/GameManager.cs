using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    private Bound[] bounds;
    private PlayerManager thePlayer;
    private CameraManager theCamera;
    private InventoryManager items;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void LoadStart()
    {
        StartCoroutine(LoadWaitCoroutine());
    }

    IEnumerator LoadWaitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        InventoryManager theInventory = FindFirstObjectByType<InventoryManager>();
        Item[] allItems = Resources.LoadAll<Item>("");

        thePlayer = FindFirstObjectByType<PlayerManager>();
        bounds = FindObjectsOfType<Bound>();
        theCamera = FindFirstObjectByType<CameraManager>();

        theCamera.target= GameObject.Find("Player");

        for(int i = 0; i < bounds.Length; i++)
        {
            if (bounds[i].boundName == thePlayer.currentMapName)
            {
                bounds[i].setBound();
                break;
            }
        }

        SaveNLoad theSaveNLoad = FindObjectOfType<SaveNLoad>();
        var data = theSaveNLoad.data;

        for (int i = 0; i < data.playerItemNames.Count; i++)
        {
            string itemName = data.playerItemNames[i];
            if (string.IsNullOrEmpty(itemName))
            {
                theInventory.items[i].isObtained = false;
                // 슬롯 빈 상태 표시 등 처리
            }
            else
            {
                // 이름으로 Item 찾아서 isObtained = true
                foreach (var item in allItems)
                {
                    if (item.itemName == itemName)
                    {
                        theInventory.items[i] = item;
                        item.isObtained = true;
                        break;
                    }
                }
            }
        }

        theInventory.UpdateSlots();
    }
}
