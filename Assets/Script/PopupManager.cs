using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;

    public GameObject popupPanel;
    public Text popupText;
    public Button confirmButton;
    public Button yesButton;
    public Button noButton;
    private int selectedIndex = 0; // 0 = Yes, 1 = No
    private System.Action onYes;
    private System.Action onNo;
    private bool isChoicePopup = false;
    private bool isPopupOpen = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        
    }

    void Start()
    {
        popupPanel.SetActive(false);// 
    }


    void Update()
    {
        // Debug.Log("1. [PopupManager] 팝업 열림, isPopupOpen: " + isPopupOpen);
        if (!popupPanel.activeSelf) return;

            // Debug.Log("1. [PopupManager] 팝업 열림, isPopupOpen: " + isPopupOpen );

        if (isChoicePopup)
        {
            if ((Menu.instance!=null&&Menu.instance.closePopup)||(SaveManager.instance!=null && SaveManager.instance.closePopup))
            {
                popupPanel.SetActive(false);
                isChoicePopup = false;
                // ▶ 팝업 닫힐 때 플레이어 움직임 다시 허용
                if (PlayerManager.instance != null)
                    PlayerManager.instance.canMove = true;
                if (Menu.instance != null) 
                    Menu.instance.closePopup = false;
                if (SaveManager.instance != null)
                    SaveManager.instance.closePopup = false;
                return;
            }

            // Debug.Log("isChoicePopup 팝업 열림, isChoicePopup: " + isChoicePopup);
            // 방향키 선택
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                selectedIndex = 0;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                selectedIndex = 1;

            HighlightButton();

            // 선택 확정
            if (Input.GetKeyDown(KeyCode.Return))
            {
                popupPanel.SetActive(false);
                isChoicePopup = false;

                if (selectedIndex == 0) onYes?.Invoke();
                else onNo?.Invoke();

                // ▶ 팝업 닫힐 때 플레이어 움직임 다시 허용
                if (PlayerManager.instance != null)
                    PlayerManager.instance.canMove = true;
            }
        }

        else if (isPopupOpen)
        {
            if (Input.GetKeyDown(KeyCode.Return)) {
                Debug.Log("show 팝업 열림, isPopupOpen: " + isPopupOpen);
                popupPanel.SetActive(false);
                isPopupOpen = false;
                Debug.Log("show 팝업 닫음, isPopupOpen: " + isPopupOpen);
                // ▶ 팝업 닫힐 때 플레이어 움직임 다시 허용
                if (PlayerManager.instance != null)
                    PlayerManager.instance.canMove = true;
                confirmButton?.onClick.Invoke();
                ClosePopupAndDelayInput();
            }
        }
    }

    public void ClosePopupAndDelayInput()
    {
        popupPanel.SetActive(false);
        isPopupOpen = false;
        StartCoroutine(DelayInputForPopup());
    }

    private IEnumerator DelayInputForPopup()
    {
        if (InventoryManager.instance != null) InventoryManager.instance.BlockInputForPopup(0.2f);  // 0.2초 입력 무시
        yield return null;
        Input.ResetInputAxes();

    }

    public void ShowPopup(string message)
    {
        Debug.Log("2. showpopup");
        popupText.text = message;
        popupPanel.SetActive(true);
        isPopupOpen = true;

        confirmButton?.gameObject.SetActive(true); // 확인 버튼만 표시
        yesButton?.gameObject.SetActive(false);
        noButton?.gameObject.SetActive(false);

        confirmButton?.onClick.RemoveAllListeners();
        /*confirmButton?.onClick.AddListener(() =>
        {
            popupPanel.SetActive(false);

            // ▶ 팝업 닫힐 때 플레이어 움직임 다시 허용
            if (PlayerManager.instance != null)
                PlayerManager.instance.canMove = true;

        });
        */
    }

    public bool IsPopupActive() // 팝업이 떠있는지 외부에서 확인할 수 있도록 함
    {
        return popupPanel.activeSelf;
    }

    public void ShowChoicePopup(string message, System.Action yesAction, System.Action noAction)
    {
        popupText.text = message;
        popupPanel.SetActive(true);

        confirmButton.gameObject.SetActive(false); // 확인 버튼 숨기고
        yesButton.gameObject.SetActive(true);      // 예 / 아니오 버튼 표시
        noButton.gameObject.SetActive(true);

        isChoicePopup = true;
        selectedIndex = 0;

        onYes = yesAction;
        onNo = noAction;

        HighlightButton(); // 시작 시 '예' 강조
    }

    private void HighlightButton()
    {
        ColorBlock yesColor = yesButton.colors;
        ColorBlock noColor = noButton.colors;

        yesColor.normalColor = (selectedIndex == 0) ? Color.white : Color.gray;
        noColor.normalColor = (selectedIndex == 1) ? Color.white : Color.gray;

        yesButton.colors = yesColor;
        noButton.colors = noColor;
    }
}
