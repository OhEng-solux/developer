using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public string characterName;
    public float speed;
    public int walkCount;
    protected int currentWalkCount;
    //speed=2.4, walkCount=20이면 한번 방향키 누를 때마다 2.4*20=48씩 이동
    //while문 사용: currentWalkCount+=1, 20될때까지

    //protected bool npcCanMove=true;
    private bool notCoroutine=false;

    protected Vector3 vector;

    public Queue<string> queue; //선입선출 자료구조
    
    public BoxCollider2D boxCollider;
    public LayerMask layerMask; //통과 불가능한 레이어 설정
    public Animator animator;

    public void Move(string _dir, int _frequency=5)
    {
        queue.Enqueue(_dir);
        if(!notCoroutine)
        {
            notCoroutine=true;
            StartCoroutine(MoveCoroutine(_dir, _frequency));
        }
    }

    IEnumerator MoveCoroutine(string _dir, int _frequency)
    {
        switch(_frequency)
        {
            case 1:
                yield return new WaitForSeconds(4f);
                break;
            case 2:
                yield return new WaitForSeconds(3f);
                break;
            case 3:
                yield return new WaitForSeconds(2f);
                break;
            case 4:
                yield return new WaitForSeconds(1f);
                break;
            case 5:
                break;
        }
        while(queue.Count!=0)
        {
            string direction=queue.Dequeue();
            //npcCanMove=false;
            vector.Set(0,0,vector.z);
            
            switch(direction)
            {
                case "UP":
                    vector.y=1f;
                    break;
                case "DOWN":
                    vector.y=-1f;
                    break;
                case "RIGHT":
                    vector.x=1f;
                    break;
                case "LEFT":
                    vector.x=-1f;
                    break;
            }

            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            while(true)
            {
                bool checkCollisionFlag=CheckCollision();
                if (checkCollisionFlag)
                {
                    animator.SetBool("Walking", false);
                    yield return new WaitForSeconds(1f);
                    break;
                }
                else
                {
                    break;
                }
            }

            animator.SetBool("Walking", true);

            boxCollider.offset = new Vector2(vector.x*0.7f*speed*walkCount, vector.y*0.7f*speed*walkCount);

            while(currentWalkCount<walkCount){
                transform.Translate(vector.x*speed,vector.y,0);
                currentWalkCount++;
                if(currentWalkCount==12)
                    boxCollider.offset=Vector2.zero;
                yield return new WaitForSeconds(0.01f);
            }

            currentWalkCount=0;
            if(_frequency!=5)
                animator.SetBool("Walking", false);
            //npcCanMove=true;
        }
        animator.SetBool("Walking", false);
        notCoroutine=false;
    }

    protected bool CheckCollision()
    {
        RaycastHit2D hit;
        //A지점에서 B지점으로 레이저를 쏜다고 했을때
        //도달하면 hit==NULL, 중간에 막히면 그 방해물을 반환한다

        Vector2 start=transform.position; //A지점: 캐릭터의 현재 위치 값
        Vector2 end=start+new Vector2(vector.x*speed*walkCount, vector.y*speed*walkCount); //B지점: 캐릭터가 이동하고자 하는 위치 값

        boxCollider.enabled=false;
        hit=Physics2D.Raycast(start, end - start, (end - start).magnitude, layerMask); //레이저발사
        boxCollider.enabled=true;

        if(hit.transform!=null)
            return true;
        return false;
    }
}
