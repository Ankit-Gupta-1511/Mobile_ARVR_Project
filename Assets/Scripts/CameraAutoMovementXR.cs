using UnityEngine;
using UnityEngine.InputSystem;

public class CardboardAutoMoveFromCamera : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    private bool isMoving = false;

    void Update()
    {
        // Toggle movement on screen tap (new Input System)
        if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
        {
            isMoving = !isMoving;
        }

        // Move in the direction the camera is facing
        if (isMoving && Camera.main != null)
        {
            Vector3 direction = Camera.main.transform.forward;
            direction.y = 0; // Prevent floating/falling
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }
}
