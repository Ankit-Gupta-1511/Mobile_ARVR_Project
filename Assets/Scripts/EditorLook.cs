using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLook : MonoBehaviour
{
    public float lookSpeed = 3f;

    void Update()
    {
        if (Application.isEditor)
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
            transform.Rotate(Vector3.up, mouseX);
            transform.Rotate(Vector3.left, mouseY);
        }
    }
}

