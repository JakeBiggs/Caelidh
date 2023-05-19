using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    public Camera main_camera;

    private void Start()
    {
        main_camera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + main_camera.transform.rotation * Vector3.back
                         , main_camera.transform.rotation * Vector3.up);
    }
}
