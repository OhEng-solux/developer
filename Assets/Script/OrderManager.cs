using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OrderManager : MonoBehaviour
{
    private PlayerManager thePlayer; // 이벤트 도중에 키 입력 처리 방지
    // private MovingObject[] characters; -> 배열은 고정된 크기를 가지므로 NPC의 개수가 변할 때마다 크기를 조정해야 함
    // -> 배열 대신 list 사용
    private List<MovingObject> characters;
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
        MovingObject[] temp = FindObjectsByType<MovingObject>(FindObjectsSortMode.None); // 씬에 있는 모든 MovingObject를 찾아서 temp 배열에 저장

        for (int i = 0; i < temp.Length; i++)
        {
            tempList.Add(temp[i]); // temp 배열의 요소를 리스트에 추가
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

    public void SetThorought(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if( _name == characters[i].characterName) // 이름이 일치하는 오브젝트 찾기
            {
                characters[i].boxCollider.enabled = false; // BoxCollider2D 비활성화
            }
        }
    }

     public void SetUnThorought(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if( _name == characters[i].characterName) // 이름이 일치하는 오브젝트 찾기
            {
                characters[i].boxCollider.enabled = true; // BoxCollider2D 활성화
            }
        }
    }

    public void SetTransparent(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if( _name == characters[i].characterName) // 이름이 일치하는 오브젝트 찾기
            {
                characters[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetUnTransparent(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if( _name == characters[i].characterName) // 이름이 일치하는 오브젝트 찾기
            {
                characters[i].gameObject.SetActive(true);
            }
        }
    }

     public void Move(string _name, string _dir)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if( _name == characters[i].characterName) // 이름이 일치하는 오브젝트 찾기
            {
                characters[i].Move(_dir);
            }
        }
    }

    public void Turn(string _name, string _dir)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if( _name == characters[i].characterName) // 이름이 일치하는 오브젝트 찾기
            {
                characters[i].animator.SetFloat("DirX", 0f);
                characters[i].animator.SetFloat("DirY", 0f);
                switch(_dir)
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
