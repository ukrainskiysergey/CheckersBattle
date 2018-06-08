using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform Target;
    public float Distance = 20f;
    public float CameraSpeed = 50f;
    public float AngleX = 0.0f;
    public float AngleY = 45.0f;

    public bool locked = false;

    void Start()
    {
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !locked)
            inUse = true;
        if (Input.GetMouseButtonUp(0))
            inUse = false;

        if (Input.GetMouseButton(0) && inUse)
        {
            AngleX += Input.GetAxis("Mouse X") * CameraSpeed * Time.deltaTime;
            AngleY -= Input.GetAxis("Mouse Y") * CameraSpeed * Time.deltaTime;
        }

        transform.position = Quaternion.Euler(AngleY, AngleX, 0) * new Vector3(0.0f, 0, -Distance) + Target.position;
        transform.LookAt(Target.position);
    }


    private bool inUse = false;
}
