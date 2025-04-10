import asyncio
import websockets
import json
from detect_shoe import detect_objects
import traceback

async def handler(websocket, path):
    print("Client connected")

    try:
        async for message in websocket:
            print("Image received")

            # Expecting a base64-encoded image string
            try:
                detections = detect_objects(message)
                print(detections)
                await websocket.send(json.dumps(detections))
            except Exception as e:
                error_msg = {'error': str(e)}
                print(e)
                traceback.print_exc()
                await websocket.send(json.dumps(error_msg))

    except websockets.ConnectionClosed:
        print("Client disconnected")

# Start server
start_server = websockets.serve(handler, "0.0.0.0", 8765)
print("WebSocket Server running at ws://localhost:8765")

asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()
