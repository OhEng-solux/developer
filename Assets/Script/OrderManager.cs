using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OrderManager : MonoBehaviour
{
    private PlayerManager thePlayer; // �̺�Ʈ ���߿� Ű �Է� ó�� ����
    private List<MovingObject> characters;
    // MovingObject[] -> �迭�� ũ�� �����Ǹ� ���� �Ұ��� ������ ����� ������, List ���
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
        MovingObject[] temp = FindObjectsOfType<MovingObject>(); // MovingObject�� �޸� ��� ��ü�� ã�Ƽ� ��ȯ�� �� (objects)

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

    // �÷��̾ �Ѱ� ����� ������ NPC�� �÷��̾��� ��ġ�� ���ؼ� ���ǹ� ����
    public void SetThorought(string _name) // �� �ձ�
    {
        for (int i = 0; i < characters.Count; i++)
        { // ����Ʈ�� ũ��� Count
            {
                if (_name == characters[i].characterName)
                {
                    characters[i].boxCollider.enabled = false;
                }
            }
        }
    }
    public void SetUnThorought(string _name) // �ٽ� ��� ���ϰ�
    {
        for (int i = 0; i < characters.Count; i++)
        { // ����Ʈ�� ũ��� Count
            {
                if (_name == characters[i].characterName)
                {
                    characters[i].boxCollider.enabled = true;
                }
            }
        }
    }

    public void SetTransparent(string _name) // ������ ���� (�������)
    {
        for (int i = 0; i < characters.Count; i++)
        { // ����Ʈ�� ũ��� Count
            {
                if (_name == characters[i].characterName)
                {
                    characters[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetUnTransparent(string _name) // ������ ���� (�ٽ� ����)
    {
        for (int i = 0; i < characters.Count; i++)
        { // ����Ʈ�� ũ��� Count
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
        { // ����Ʈ�� ũ��� Count
            {
                if (_name == characters[i].characterName)
                {
                    // ���� �Լ� ����
                    characters[i].Move(_dir);
                }
            }
        }
    }

    public void Turn(string _name, string _dir)
    {
        for (int i = 0; i < characters.Count; i++)
        { // ����Ʈ�� ũ��� Count
            {
                if (_name == characters[i].characterName)
                {
                    // �ʱ�ȭ
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
