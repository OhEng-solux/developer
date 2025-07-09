using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    #region Singleton -> 숨김 기능이 있는 주석

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject); // Keep this instance across scenes
            instance = this;
        }
        else
        {
            Destroy(this.gameObject); // Ensure only one instance exists
        }
    }
    #endregion Singleton

    public Text text;
    public SpriteRenderer rendererSprite;
    public SpriteRenderer rendererDialogueWindow;

    private List<string> listSentences;
    private List<Sprite> listSprites;
    private List<Sprite> listDialogueWindows; // -> Dialogue.cs 배열에 있는 애들을 다 이 리스트에 집어넣을 예정

    private int count; // 대화 진행 상황 카운트

    public Animator animSprite;
    public Animator animDialogueWindow;

    public string typeSound;
    public string enterSound;

    private AudioManager theAudio;
    private OrderManager theOrder;

    public bool talking = false; // 대화 중인지 여부를 나타내는 변수
    private bool keyActivated = false;



    void Start()
    {
        count = 0; // 대화 진행 상황 카운트 초기화
        text.text = ""; // 대화 텍스트 초기화
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        listSentences = new List<string>();
        theAudio = FindAnyObjectByType<AudioManager>(); // AudioManager 인스턴스 찾기
        theOrder = FindAnyObjectByType<OrderManager>(); // OrderManager 인스턴스 찾기
    }

    // ShowDialogue는 대화 내용을 표시하는 메서드입니다.
    public void ShowDialogue(Dialogue dialogue)
    {
        talking = true; // 대화 중

        theOrder.NotMove(); // OrderManager의 NotMove 메서드를 호출하여 플레이어의 이동을 막음
        for(int i=0; i<dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]); // 대화 문장 추가
            listSprites.Add(dialogue.sprites[i]); // 대화 문장 추가
            listDialogueWindows.Add(dialogue.dialogueWindows[i]); // 대화 문장 추가
        }
        animSprite.SetBool("Appear", true); // 대화창 애니메이션 시작
        animDialogueWindow.SetBool("Appear", true); // 대화창 애니메이션 시작
        StartCoroutine(StartDialogueCoroutine()); //코루틴을 시작할 때 대화가 진행된다는 뜻

    }

    public void ExitDialogue()
    {
        count = 0; // 대화 진행 상황 카운트 초기화
        text.text = ""; // 대화 텍스트 초기화
        listSentences.Clear(); // 대화 문장 리스트 초기화
        listSprites.Clear(); // 대화 스프라이트 리스트 초기화
        listDialogueWindows.Clear(); // 대화창 이미지 리스트 초기화
        animSprite.SetBool("Appear", false); // 대화창 애니메이션 종료
        animDialogueWindow.SetBool("Appear", false); // 대화창 애니메이션 종료
        talking = false; // 대화 중이 아님
        theOrder.Move(); // OrderManager의 Move 메서드를 호출하여 플레이어의 이동을 허용
    }

    IEnumerator StartDialogueCoroutine()
    {
        if(count > 0) 
        {
            if (listDialogueWindows[count] != listDialogueWindows[count-1]) 
            // 대사 창이 다를 경우 -> sprite와 dialogueWindow 교체
            {
                animSprite.SetBool("Change", true); // 스프라이트 변경 애니메이션 시작
                animDialogueWindow.SetBool("Appear", false); // 대화창 변경 애니메이션 시작
                yield return new WaitForSeconds(0.2f); // 0.1초 대기
                rendererDialogueWindow.sprite = listDialogueWindows[count]; // 대화창 이미지 변경
                rendererSprite.sprite = listSprites[count]; // 스프라이트로 변경           
                animDialogueWindow.SetBool("Appear", true); // 대화창 변경 애니메이션 종료
                animSprite.SetBool("Change", false); // 스프라이트 변경 애니메이션 종료
            }
            else // 대사 창이 같을 경우 -> sprite만 교체
            {
                if(listSprites[count] != listSprites[count-1]) // 만약 현재 스프라이트가 이전 스프라이트와 다르면
                {
                    animSprite.SetBool("Change", true); // 스프라이트 변경 애니메이션 시작
                    yield return new WaitForSeconds(0.1f); // 0.1초 대기
                    rendererSprite.sprite = listSprites[count]; // 스프라이트로 변경
                    animSprite.SetBool("Change", false); // 스프라이트 변경 애니메이션 종료
                }
                else // 스프라이트도 같을 경우 -> 그냥 잠깐 대기
                {
                    yield return new WaitForSeconds(0.05f); // 대기 시간
                }
            }
        }

        else // 처음 대화가 시작될 때는 count==0
        {
            rendererDialogueWindow.sprite = listDialogueWindows[count]; // 대화창 이미지 변경
            rendererSprite.sprite = listSprites[count]; // 스프라이트로 변경       
        }

        keyActivated = true; // z키 활성화

        // 대화를 위한 for문
        for (int i = 0; i < listSentences[count].Length; i++)
        {
            text.text += listSentences[count][i]; // 대화 문장 한 글자씩 출력
            if(i % 7 ==1)
            {
                theAudio.Play(typeSound); // 글자 출력 시 타이핑 사운드 재생
            }
            yield return new WaitForSeconds(0.01f); // 0.1초 대기
        }
    }

    // Update is called once per frame
    void Update() // 사용자가 z키를 누르면 다음 대화문장으로 넘어가게끔 처리
    {
        if (talking && keyActivated) 
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                keyActivated = false; // z키 비활성화
                count++; // 대화 진행 상황 카운트 증가
                text.text = ""; // 대화 텍스트 초기화
                theAudio.Play(enterSound); // 엔터 사운드 재생

                if (count == listSentences.Count) // 대화 문장이 끝나면
                {
                    StopAllCoroutines(); // 모든 코루틴 중지
                    ExitDialogue(); // 대화 종료 메서드 호출
                }
                else
                {
                    StopAllCoroutines(); // 현재 진행 중인 코루틴 중지
                    StartCoroutine(StartDialogueCoroutine()); // 다음 대화 문장 출력 시작
                }
            }
        }
    }
}
