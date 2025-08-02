using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    private PolygonCollider2D bound;
    public string boundName;//바운드 이름 불러오기
    private CameraManager theCamera;

    public Transform taejuSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        bound = GetComponent<PolygonCollider2D>();
        Debug.Log("Bound " + bound);
        var theCamera = FindFirstObjectByType<CameraManager>();
        theCamera.SetBound(bound);
    }
    public void setBound()
    {
        if (theCamera!=null) {
            theCamera.SetBound(bound); 
        }
    }

    public Vector3 GetTaejuSpawnPosition()
    {
        // 씬에서 인스펙터로 직접 spawn point를 지정할 수 있음!
        if (taejuSpawnPoint != null)
            return taejuSpawnPoint.position;
        else
            return this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {

    }
}