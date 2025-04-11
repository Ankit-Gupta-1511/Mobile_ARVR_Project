using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFeed : MonoBehaviour
{
    public RawImage rawImage;
    [HideInInspector] public WebCamTexture webcamTexture;
    public Renderer quadRenderer;

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

        if (quadRenderer != null)
        {
            quadRenderer.material.mainTexture = webcamTexture;
        }

        StartCoroutine(WaitForCameraAndPositionQuad());

        AdjustRawImageAspect();

        
    }

    IEnumerator WaitForCameraAndPositionQuad()
    {
        Camera cam = Camera.main;

        while (webcamTexture.width <= 16)
            yield return null;

        yield return new WaitForEndOfFrame();

        Debug.Log($"Actual webcam resolution: {webcamTexture.width} x {webcamTexture.height}");
        Debug.Log($"Rotation: {webcamTexture.videoRotationAngle}");
        Debug.Log($"Mirrored: {webcamTexture.videoVerticallyMirrored}");


        float screenRatio = (float)Screen.width / Screen.height;

        // Attach to camera
        Transform quadTransform = quadRenderer.transform;

        quadTransform.SetParent(cam.transform);

        float distance = 1f;
        quadTransform.localPosition = new Vector3(0f, 0f, distance);

        float videoRotationAngle = webcamTexture.videoRotationAngle;
        quadTransform.localRotation = Quaternion.AngleAxis(videoRotationAngle, Vector3.forward);


        float frustumHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        Vector3 quadScale = Vector3.one;

        if (videoRotationAngle == 0 || videoRotationAngle == 180)
        {
            float textureRatio = (float)webcamTexture.width / webcamTexture.height;

            if (screenRatio > textureRatio)
            {
                float scaleMultiplier = screenRatio / textureRatio;
                float width = textureRatio * frustumHeight * scaleMultiplier;
                float height = frustumHeight * (webcamTexture.videoVerticallyMirrored ? -1f : 1f) * scaleMultiplier;
                quadScale = new Vector3(width, height, 1f);
            }
            else
            {
                float width = textureRatio * frustumHeight;
                quadScale = new Vector3(width, frustumHeight * (webcamTexture.videoVerticallyMirrored ? -1f : 1f), 1f);
            }
        }
        else
        {
            float textureRatio = (float)webcamTexture.height / webcamTexture.width;

            if (screenRatio > textureRatio)
            {
                float scaleMultiplier = screenRatio / textureRatio;
                float width = frustumHeight * scaleMultiplier;
                float height = width * (webcamTexture.videoVerticallyMirrored ? 1f : -1f);
                quadScale = new Vector3(width, height, 1f);
            }
            else
            {
                float width = textureRatio * frustumHeight;
                quadScale = new Vector3(frustumHeight, width * (webcamTexture.videoVerticallyMirrored ? 1f : -1f), 1f);
            }
        }

        quadTransform.localScale = quadScale;

        quadRenderer.material.mainTexture = webcamTexture;
    }


    void AdjustRawImageAspect()
    {
        AspectRatioFitter fitter = rawImage.GetComponent<AspectRatioFitter>();
        if (fitter != null)
        {
            fitter.aspectRatio = (float)webcamTexture.width / webcamTexture.height;
        }
    }

}
