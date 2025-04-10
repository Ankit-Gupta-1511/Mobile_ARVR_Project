using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GazeActivator : MonoBehaviour
{
    public float gazeDuration = 2f;
    private float gazeTimer = 0f;
    private GameObject currentTarget;

    private PlayerInputActions inputActions;
    private bool tapTriggered;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Gameplay.Tap.performed += ctx => tapTriggered = true;
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Shoe"))
            {
                if (hitObject != currentTarget)
                {
                    currentTarget = hitObject;
                    gazeTimer = 0f;
                }

                gazeTimer += Time.deltaTime;

                if (gazeTimer >= gazeDuration && tapTriggered)
                {
                    ShoeData shoeData = hitObject.GetComponent<ShoeData>();
                    if (shoeData != null)
                    {
                        Debug.Log($"Gaze + tap triggered on shoe: {shoeData.shoeName}");
                        FindObjectOfType<XRSceneManager>().LoadARScene();
                        tapTriggered = false;
                    }

                    tapTriggered = false;
                    gazeTimer = 0f; // Optional: reset after triggering
                }
            }
            else
            {
                ResetGaze();
            }
        }
        else
        {
            ResetGaze();
        }
    }

    void ResetGaze()
    {
        gazeTimer = 0f;
        currentTarget = null;
        tapTriggered = false;
    }
}
