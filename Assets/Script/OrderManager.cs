using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OrderManager : MonoBehaviour
{
    private PlayerManager thePlayer; // 이벤트 도중에 키 입력 처리 방지
    private List<MovingObject> characters;
    // MovingObject[] -> 배열의 크기 고정되면 변경 불가한 문제가 생기기 때문에, List 사용
    // Add(), Remove(), Clear()

    void Start()
    {
        thePlayer = FindAnyObjectByType<PlayerManager>();
        Debug.Log("[OrderManager] PlayerManager: " + thePlayer);
    }


    public void PreLoadCharacter()
    {
        characters = ToList();
    }

    public List<MovingObject> ToList()
    {
        List<MovingObject> tempList = new List<MovingObject>();
        MovingObject[] temp = FindObjectsOfType<MovingObject>(); // MovingObject가 달린 모든 객체를 찾아서 반환해 줌 (objects)

        for (int i = 0; i < temp.Length; i++)
        {
            tempList.Add(temp[i]);
        }

        return tempList;
    }

    public void NotMove()
    {
        thePlayer.notMove = true;
    }

    public void Move()
    {
        thePlayer.notMove = false;
    }

    // 플레이어를 쫓게 만들고 싶으면 NPC와 플레이어의 위치를 비교해서 조건문 설정
    public void SetThorought(string _name) // 벽 뚫기
    {
        for (int i = 0; i < characters.Count; i++)
        { // 리스트의 크기는 Count
            {
                if (_name == characters[i].characterName)
                {
                    characters[i].boxCollider.enabled = false;
                }
            }
        }
    }
    public void SetUnThorought(string _name) // 다시 통과 못하게
    {
        for (int i = 0; i < characters.Count; i++)
        { // 리스트의 크기는 Count
            {
                if (_name == characters[i].characterName)
                {
                    characters[i].boxCollider.enabled = true;
                }
            }
        }
    }

    public void SetTransparent(string _name) // 투명도 조절 (사라지게)
    {
        for (int i = 0; i < characters.Count; i++)
        { // 리스트의 크기는 Count
            {
                if (_name == characters[i].characterName)
                {
                    characters[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetUnTransparent(string _name) // 투명도 조절 (다시 생성)
    {
        for (int i = 0; i < characters.Count; i++)
        { // 리스트의 크기는 Count
            {
                if (_name == characters[i].characterName)
                {
                    characters[i].gameObject.SetActive(true);
                }
            }
        }
    }

    public void Move(string _name, string _dir)
    {
        for (int i = 0; i < characters.Count; i++)
        { // 리스트의 크기는 Count
            {
                if (_name == characters[i].characterName)
                {
                    // 무브 함수 실행
                    characters[i].Move(_dir);
                }
            }
        }
    }

    public void Turn(string _name, string _dir)
    {
        for (int i = 0; i < characters.Count; i++)
        { // 리스트의 크기는 Count
            {
                if (_name == characters[i].characterName)
                {
                    // 초기화
                    characters[i].animator.SetFloat("DirX", 0f);
                    characters[i].animator.SetFloat("DirY", 0f);
                    switch (_dir)
                    {
                        case "UP":
                            characters[i].animator.SetFloat("DirY", 1f);
                            break;
                        case "DOWN":
                            characters[i].animator.SetFloat("DirY", -1f);
                            break;
                        case "LEFT":
                            characters[i].animator.SetFloat("DirX", -1f);
                            break;
                        case "RIGHT":
                            characters[i].animator.SetFloat("DirX", 1f);
                            break;
                    }
                }
            }
        }
    }
}
