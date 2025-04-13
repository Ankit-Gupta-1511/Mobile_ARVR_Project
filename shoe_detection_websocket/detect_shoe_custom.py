from PIL import Image
from io import BytesIO
import numpy as np
import tensorflow as tf
import cv2
import base64

custom_model_path = "/Users/privy/Downloads/detection_model_saved"
imported = tf.saved_model.load(custom_model_path)
custom_model = imported.signatures["serving_default"]

def detect_objects_custom(base64_string):
    image_data = base64.b64decode(base64_string)
    image = Image.open(BytesIO(image_data)).convert("RGB")
    image_array = np.array(image)

    # model prediction
    resized_image = cv2.resize(image_array, (224,224))
    resized_image = resized_image/255
    resized_image = np.expand_dims(resized_image, axis=0)
    resized_image = tf.cast(resized_image, dtype=tf.float32)
    detection = custom_model(resized_image)['output_0'].numpy()[0]

    # prepare 2d landmark
    detections = []
    landmark_2d = []
    h, w, _ = image_array.shape
    i = 0
    #image_copy = image.copy()
    while i< len(detection):
        x = int(detection[i] * w)
        y = int(detection[i+1] * h)
        landmark_2d.append({"x": x, "y": y})
        #cv2.circle(image_copy, (x, y), 63, (255,0,0), -1)
        i += 2
    detections.append(
        {
            "image_height": h,
            "image_width": w,
            "landmarks_2d": landmark_2d
        })
    #cv2.imwrite("output.png", cv2.cvtColor(image_copy, cv2.COLOR_RGB2BGR))
    return detections