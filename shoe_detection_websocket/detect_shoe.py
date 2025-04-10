import mediapipe as mp
from PIL import Image
from io import BytesIO
import numpy as np
import base64
import math

from scipy.spatial.transform import Rotation as R  # For matrix to quaternion

mp_objectron = mp.solutions.objectron

def detect_objects(base64_string):
    # Decode base64 to image
    image_data = base64.b64decode(base64_string)
    image = Image.open(BytesIO(image_data)).convert("RGB")
    image_array = np.array(image)

    # Initialize Objectron
    with mp_objectron.Objectron(
        static_image_mode=True,
        max_num_objects=1,
        min_detection_confidence=0.5,
        model_name="Shoe",
    ) as objectron:
        results = objectron.process(image_array)

    if not results or not results.detected_objects:
        print("No shoes detected.")
        return {
            "position": [0, 0, -10],
            "rotation": [0, 0, 0, 1],
            "scale": [0.01, 0.01, 0.01]
        }

    detected_object = results.detected_objects[0]

    # Extract translation vector
    position = list(detected_object.translation)

    # Extract rotation matrix (3x3) and convert to quaternion
    rotation_matrix = np.array(detected_object.rotation)
    try:
        quaternion = R.from_matrix(rotation_matrix).as_quat()
        rotation = quaternion.tolist()  # [x, y, z, w]
    except Exception as e:
        print("Rotation matrix conversion failed:", e)
        rotation = [0, 0, 0, 1]

    # Estimate scale from bounding box (landmarks_3d)
    x_vals = [lm.x for lm in detected_object.landmarks_3d.landmark]
    y_vals = [lm.y for lm in detected_object.landmarks_3d.landmark]
    z_vals = [lm.z for lm in detected_object.landmarks_3d.landmark]

    scale = [
        max(x_vals) - min(x_vals),
        max(y_vals) - min(y_vals),
        max(z_vals) - min(z_vals)
    ]

    return {
        "position": position,
        "rotation": rotation,
        "scale": scale
    }
