using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerController : MonoBehaviour
{
    Camera mcam;
    float scrollSpeed = 10;
    float rotSpeed = 40;

    void Start()
    {
        mcam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKey("q"))
        {
            mcam.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), rotSpeed * Time.deltaTime);
        }
        if (Input.GetKey("e"))
        {
            mcam.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), -rotSpeed * Time.deltaTime);
        }

        Vector3 pos = mcam.transform.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        Vector3 dir = Vector3.zero - mcam.transform.position;
        dir *= scroll;
        mcam.transform.Translate(dir.normalized, Space.World);
    }
}
