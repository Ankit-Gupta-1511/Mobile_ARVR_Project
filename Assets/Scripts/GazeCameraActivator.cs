using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
            if (hit.collider.CompareTag("Treasure"))
            {

                if (hit.collider.gameObject != currentTarget)
                {
                    currentTarget = hit.collider.gameObject;
                    gazeTimer = 0f;
                }

                gazeTimer += Time.deltaTime;
                if (gazeTimer >= gazeDuration)
                {
                    Debug.Log("Treasure selected via gaze + tap.");
                    FindObjectOfType<XRSceneManager>().LoadARScene();
                    tapTriggered = false;
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
