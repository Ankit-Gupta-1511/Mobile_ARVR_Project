using System;
using System.Text;
using UnityEngine;
using NativeWebSocket;
using System.Collections;

[Serializable]
public class ShoePose
{
    public float[] position;
    public float[] rotation;
    public float[] scale;
}

public class ShoePoseReceiver : MonoBehaviour
{
    [Header("Dependencies")]
    public CameraFeed cameraFeedScript;         // Drag the GameObject with CameraFeed.cs
    public GameObject shoeModel;                // Assign your shoe model prefab
    public Camera arUICamera;                   // Camera that renders the canvas

    [Header("Networking")]
    public string serverIP = "192.168.0.108";    // Replace with your Python server IP
    public int port = 8765;
    public float updateInterval = 0.5f;

    public Transform shoeAnchor;
    public float activationDistance = 0.2f;

    private NativeWebSocket.WebSocket websocket;
    private WebCamTexture cam;
    private float timer;
    private bool isCameraReady;

    async void Start()
    {
        if (cameraFeedScript == null)
        {
            Debug.LogError("CameraFeed script not assigned!");
            return;
        }

        StartCoroutine(WaitForCameraFeed());

        websocket = new NativeWebSocket.WebSocket($"ws://{serverIP}:{port}");

        websocket.OnOpen += () => Debug.Log("WebSocket connected.");
        websocket.OnMessage += (bytes) =>
        {
            string json = Encoding.UTF8.GetString(bytes);
            ShoePose pose = JsonUtility.FromJson<ShoePose>(json);

            if (pose.position.Length == 3 && pose.rotation.Length == 4 && pose.scale.Length == 3)
            {
                // Flip Z for position
                Vector3 pos = new Vector3(pose.position[0], pose.position[1], -pose.position[2]);

                // Flip Z and W for rotation to convert OpenGL to Unity (left-handed)
                Quaternion rot = new Quaternion(
                    pose.rotation[0],
                    pose.rotation[1],
                    -pose.rotation[2],
                    -pose.rotation[3]
                );
                Vector3 scl = new Vector3(pose.scale[0], pose.scale[1], pose.scale[2]);

                UpdateShoePose(pos, rot, scl);
            }
        };

        websocket.OnError += (e) => Debug.LogError("WebSocket error: " + e);
        websocket.OnClose += (e) => Debug.LogWarning("WebSocket closed.");

        await websocket.Connect();
    }

    IEnumerator WaitForCameraFeed()
    {
        float timeout = 5f;
        float timer = 0f;

        while ((cameraFeedScript.webcamTexture == null || !cameraFeedScript.webcamTexture.isPlaying) && timer < timeout)
        {
            Debug.Log("Waiting for CameraFeed to initialize...");
            timer += Time.deltaTime;
            yield return null;
        }

        if (cameraFeedScript.webcamTexture != null && cameraFeedScript.webcamTexture.isPlaying)
        {
            cam = cameraFeedScript.webcamTexture;
            StartCoroutine(WaitForCameraReady());
        }
        else
        {
            Debug.LogError("CameraFeed webcamTexture failed to initialize.");
        }
    }



    IEnumerator WaitForCameraReady()
    {
        float timeout = 5f;
        float timer = 0f;

        while ((cam.width <= 16 || cam.height <= 16 || !cam.isPlaying) && timer < timeout)
        {
            Debug.Log($"Waiting for cam... width: {cam.width}, height: {cam.height}, playing: {cam.isPlaying}");
            timer += Time.deltaTime;
            yield return null;
        }

        if (cam.isPlaying)
        {
            Debug.Log($"Camera ready: {cam.width}x{cam.height}");
            isCameraReady = true;
        }
        else
        {
            Debug.LogError("WebCamTexture failed to initialize.");
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif

        if (!isCameraReady) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0;
            CaptureAndSendFrame();
        }
    }

    void CaptureAndSendFrame()
    {
        RenderTexture rt = new RenderTexture(cam.width, cam.height, 0);
        Graphics.Blit(cam, rt);

        RenderTexture.active = rt;
        Texture2D snap = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        snap.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        snap.Apply();

        byte[] jpg = snap.EncodeToJPG(50);
        string base64 = Convert.ToBase64String(jpg);

        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            websocket.SendText(base64);
        }

        RenderTexture.active = null;
        rt.Release();
        Destroy(rt);
        Destroy(snap);
    }

    /*void UpdateShoePose(Vector3 cameraSpacePos, Quaternion rotation, Vector3 scale)
    {
        Debug.Log($"[Pose Debug] Pos: {cameraSpacePos}, Rot: {rotation.eulerAngles}, Scale: {scale}");

        // Sanitize input
        if (cameraSpacePos == Vector3.zero || cameraSpacePos.z < 0)
        {
            shoeModel.SetActive(false);
            return;
        }

        cameraSpacePos.z = Mathf.Max(cameraSpacePos.z, 0.5f); // push in front of camera if needed
        Vector3 worldPos = Camera.main.transform.TransformPoint(cameraSpacePos);

        shoeModel.transform.position = worldPos;
        shoeModel.transform.rotation = Camera.main.transform.rotation * rotation;

        shoeModel.transform.localScale = new Vector3(
            Mathf.Max(scale.x, 0.02f),
            Mathf.Max(scale.y, 0.02f),
            Mathf.Max(scale.z, 0.02f)
        );

        shoeModel.SetActive(true);
        Debug.Log("Corrected World Shoe Pos: " + worldPos);
    }*/

    void UpdateShoePose(Vector3 cameraSpacePos, Quaternion rotation, Vector3 scale)
    {
        Debug.Log($"[Pose Debug] Pos: {cameraSpacePos}, Rot: {rotation.eulerAngles}, Scale: {scale}");

        // Sanitize input
        if (cameraSpacePos == Vector3.zero || cameraSpacePos.z < 0)
        {
            /*shoeModel.SetActive(false);*/
            return;
        }

        cameraSpacePos.z = Mathf.Max(cameraSpacePos.z, 0.5f); // push in front of camera if needed
        Vector3 worldPos = Camera.main.transform.TransformPoint(cameraSpacePos);

        // Check if the detected foot is close enough to the anchor
        float distanceToAnchor = Vector3.Distance(worldPos, shoeAnchor.position);
        if (distanceToAnchor > activationDistance)
        {
            /*shoeModel.SetActive(false);*/
            return;
        }

        // Snap shoe to anchor
        shoeModel.transform.position = shoeAnchor.position;

        shoeModel.transform.localScale = new Vector3(
            Mathf.Max(scale.x, 0.06f),
            Mathf.Max(scale.y, 0.06f),
            Mathf.Max(scale.z, 0.06f)
        );
        Debug.Log($"[Pose Debug] Pos: {cameraSpacePos}, Rot: {rotation.eulerAngles}, Scale: {shoeModel.transform.localScale}");
        shoeModel.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        if (shoeAnchor != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(shoeAnchor.position, activationDistance);
        }
    }




    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}
