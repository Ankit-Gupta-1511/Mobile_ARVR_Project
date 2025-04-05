using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToVR : MonoBehaviour
{
    public void GoBack()
    {
        FindObjectOfType<XRSceneManager>().LoadVRScene();
    }
}
