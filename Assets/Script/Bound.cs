using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    private PolygonCollider2D bound;
    public string boundName;//바운드 이름 불러오기
    private CameraManager theCamera;

    // Start is called before the first frame update
    void Start()
    {
        bound = GetComponent<PolygonCollider2D>();
        var theCamera = FindFirstObjectByType<CameraManager>();
        theCamera.SetBound(bound);
    }
    public void setBound()
    {
        if (theCamera!=null) {
            theCamera.SetBound(bound); 
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}