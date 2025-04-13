using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class DynamicSHLightingWithProbes : MonoBehaviour
{
    [Header("Server Config")]
    public string shApiUrl = "http://192.168.0.108/sh"; // Replace with actual SH endpoint
    public float updateInterval = 1f; // Fetch new SH every 1 second

    void Start()
    {
        StartCoroutine(UpdateSHLightingRoutine());
    }

    IEnumerator UpdateSHLightingRoutine()
    {
        while (true)
        {
            UnityWebRequest req = UnityWebRequest.Get(shApiUrl);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                // Assume JSON format: { "sh": [27 floats] }
                SHResponse response = JsonUtility.FromJson<SHResponse>(req.downloadHandler.text);
                ApplySHToLightProbes(response.sh);
            }
            else
            {
                Debug.LogWarning("Failed to fetch SH data: " + req.error);
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    void ApplySHToLightProbes(List<float> coeffs)
    {
        if (coeffs.Count != 27)
        {
            Debug.LogError("Invalid SH data: Expected 27 floats, got " + coeffs.Count);
            return;
        }

        // Create SH object from coefficients
        SphericalHarmonicsL2 sh = new SphericalHarmonicsL2();

        for (int channel = 0; channel < 3; channel++) // R, G, B
        {
            for (int i = 0; i < 9; i++)
            {
                sh[channel, i] = coeffs[channel * 9 + i];
            }
        }

        // Apply to all baked light probes in the scene
        var probes = LightmapSettings.lightProbes.bakedProbes;

        for (int i = 0; i < probes.Length; i++)
        {
            probes[i] = sh; // Same SH applied to every probe
        }

        LightmapSettings.lightProbes.bakedProbes = probes;

        Debug.Log("Updated light probes with dynamic SH coefficients.");
    }

    [System.Serializable]
    public class SHResponse
    {
        public List<float> sh;
    }
}
