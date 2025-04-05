using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFeed : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webcamTexture;

    void Start()
    {
        // Find the back camera
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                webcamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        if (webcamTexture == null)
        {
            Debug.LogWarning("Back camera not found. Using default.");
            webcamTexture = new WebCamTexture();
        }

        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }
}
