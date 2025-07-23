using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuzzleButton : MonoBehaviour
{
    public string shape; // ��, ��, ��, ��
    private PuzzleManager manager;

    private void Start()
    {
        manager = FindObjectOfType<PuzzleManager>();
        GetComponent<Button>().onClick.AddListener(() => manager.ButtonPressed(shape));
    }
}
