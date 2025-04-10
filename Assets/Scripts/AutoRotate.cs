using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // Rotate around Y-axis
    public float rotationSpeed = 20f;         // Degrees per second

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
    }
}
