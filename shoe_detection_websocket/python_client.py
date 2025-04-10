# test_client.py
import asyncio
import websockets
import base64


async def send_image():
    uri = "ws://localhost:8765"

    # Load and encode image to base64
    with open("download.jpeg", "rb") as img_file:
        base64_img = base64.b64encode(img_file.read()).decode("utf-8")

    async with websockets.connect(uri) as websocket:
        await websocket.send(base64_img)
        response = await websocket.recv()
        print("Detections:", response)


asyncio.run(send_image())
