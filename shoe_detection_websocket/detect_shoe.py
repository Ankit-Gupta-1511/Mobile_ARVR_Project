import mediapipe as mp
from PIL import Image
from io import BytesIO
import numpy as np
import websockets
import base64

mp_drawing = mp.solutions.drawing_utils
mp_objectron = mp.solutions.objectron


def detect_objects(base64_string):
    image_data = base64.b64decode(base64_string)
    image = Image.open(BytesIO(image_data)).convert("RGB")
    image_array = np.array(image)
    width, height = image.size

    # detect shoes
    with mp_objectron.Objectron(
        static_image_mode=True,
        max_num_objects=5,
        min_detection_confidence=0.5,
        model_name="Shoe",
    ) as objectron:
        results = objectron.process(image_array)

    # post process detections
    detections = []
    for detected_object in results.detected_objects:
        # get translation
        translation_dict = {}
        for i, val in enumerate(detected_object.translation):
            translation_dict[str(i)] = val

        detections.append(
            {
                "image_height": height,
                "image_width": width,
                "landmarks_2d": [
                    {"x": landmark.x, "y": landmark.y}
                    for landmark in detected_object.landmarks_2d.landmark
                ],
                "landmarks_3d": [
                    {"x": landmark.x, "y": landmark.y, "z": landmark.z}
                    for landmark in detected_object.landmarks_3d.landmark
                ],
                "rotation": [
                    {"col0": x[0], "col1": x[1], "col2": x[2]}
                    for x in detected_object.rotation
                ],
                "translation": translation_dict,
            }
        )

    return detections
