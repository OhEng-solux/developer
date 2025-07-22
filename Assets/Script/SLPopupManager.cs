using UnityEngine;
using UnityEngine.UI;

public class SLPopupManager : MonoBehaviour
{
    public static SLPopupManager instance;

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

        popupPanel.SetActive(false);
    }

    void Update()
    {
        if (!popupPanel.activeSelf) return;

        if (isChoicePopup)
        {
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
            }
        }

        else if (isPopupOpen && Input.GetKeyDown(KeyCode.Return))
        {
            popupPanel.SetActive(false);
            isPopupOpen = false;
        }
    }

    public void ShowPopup(string message)
    {
        popupText.text = message;
        popupPanel.SetActive(true);
        isPopupOpen = true;

        confirmButton?.gameObject.SetActive(true); // 확인 버튼만 표시
        yesButton?.gameObject.SetActive(false);
        noButton?.gameObject.SetActive(false);

        confirmButton?.onClick.RemoveAllListeners();
        confirmButton?.onClick.AddListener(() =>
        {
            popupPanel.SetActive(false);
        });
    }

    public bool IsPopupActive() //팝업이 떠있는지 외부에서 확인할 수 있도록 함
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
