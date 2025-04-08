import cv2
import mediapipe as mp
from flask import Flask, request, jsonify
from PIL import Image
import io
import numpy as np

app = Flask(__name__)

mp_drawing = mp.solutions.drawing_utils
mp_objectron = mp.solutions.objectron

app = Flask(__name__)


@app.route("/detect", methods=["POST"])
def detect_objects():
    if "image" not in request.files:
        return jsonify({"error": "No image uploaded"}), 400

    file = request.files["image"]
    image = Image.open(file.stream).convert("RGB")
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

    return jsonify({"detections": detections})


if __name__ == "__main__":
    app.run(debug=True)
