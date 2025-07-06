using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bound : MonoBehaviour
{
    private PolygonCollider2D Bound;

    private CameraManager theCamera;

    // Start is called before the first frame update
    void Start()
    {
        Bound = GetComponent<PolygonCollider2D>();
        var theCamera = FindFirstObjectByType<CameraManager>();
        theCamera.SetBound(Bound);
    }

    // Update is called once per frame
    void Update()
    {

    }
}