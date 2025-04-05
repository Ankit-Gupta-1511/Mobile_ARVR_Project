using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using System.Collections;

public class XRSceneManager : MonoBehaviour
{
    public string vrSceneName = "VRShowroom";
    public string arSceneName = "ARCameraScene";

    public bool isARScene; // Set in Inspector for each scene

    void Start()
    {
        if (isARScene)
            StartCoroutine(DisableXR()); // Turn off XR for AR scene
        else
            StartCoroutine(EnableXR()); // Ensure XR is active for VR scene
    }

    public void LoadARScene()
    {
        StartCoroutine(SwitchToAR());
    }

    public void LoadVRScene()
    {
        StartCoroutine(SwitchToVR());
    }

    IEnumerator SwitchToAR()
    {
        yield return DisableXR();
        SceneManager.LoadScene(arSceneName);
    }

    IEnumerator SwitchToVR()
    {
        yield return EnableXR();
        SceneManager.LoadScene(vrSceneName);
    }

    IEnumerator DisableXR()
    {
        Debug.Log("[XRSceneManager] Disabling XR...");
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            Debug.Log("[XRSceneManager] XR Disabled.");
        }
        yield return null;
    }

    IEnumerator EnableXR()
    {
        Debug.Log("[XRSceneManager] Enabling XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("[XRSceneManager] Failed to initialize XR.");
            yield break;
        }
        XRGeneralSettings.Instance.Manager.StartSubsystems();
        Debug.Log("[XRSceneManager] XR Enabled.");
    }
}
