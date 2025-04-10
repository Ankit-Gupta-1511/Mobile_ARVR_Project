using System;
using System.Collections;
using System.Text;
using UnityEngine;
using NativeWebSocket;

public class MobileCamWebSocketClient : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private WebSocket websocket;

    async void Start()
    {
        // Initialize and start the webcam
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();

        // Connect to WebSocket server
        websocket = new WebSocket("ws://localhost:8765");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);
            // TODO: parse JSON & visualize results
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("WebSocket Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed");
        };

        await websocket.Connect();
    }

    public void CaptureAndSendImage()
    {
        Texture2D snap = new Texture2D(webCamTexture.width, webCamTexture.height);
        snap.SetPixels32(webCamTexture.GetPixels32());
        snap.Apply();

        byte[] jpgBytes = snap.EncodeToJPG();
        string base64Image = Convert.ToBase64String(jpgBytes);

        if (websocket.State == WebSocketState.Open)
        {
            websocket.SendText(base64Image);
            Debug.Log("Image sent to server.");
        }

        Destroy(snap);
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif

        // Trigger capture (test key, or call from UI)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaptureAndSendImage();
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
