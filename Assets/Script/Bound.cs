using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bound : MonoBehaviour
{
    private PolygonCollider2D Bound;
    public string boundName;//바운드 이름 불러오기
    private CameraManager theCamera;

    // Start is called before the first frame update
    void Start()
    {
        Bound = GetComponent<PolygonCollider2D>();
        var theCamera = FindFirstObjectByType<CameraManager>();
        theCamera.SetBound(Bound);
    }
    public void setBound()
    {
        if (theCamera!=null) {
            theCamera.SetBound(Bound); 
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}