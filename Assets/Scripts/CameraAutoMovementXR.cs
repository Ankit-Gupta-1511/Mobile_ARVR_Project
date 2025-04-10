using UnityEngine;
using UnityEngine.InputSystem;

public class CardboardMoveWithController : MonoBehaviour
{
    public Camera vrCamera;
    public float moveSpeed = 2f;
    private bool isMoving = false;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
        {
            isMoving = !isMoving;
        }

        if (isMoving && vrCamera != null)
        {
            Vector3 direction = vrCamera.transform.forward;
            direction.y = 0;
            direction.Normalize();
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }
    }
}
